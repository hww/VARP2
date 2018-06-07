using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace VARP.Profile
{

    using Name = VARP.DataStructures.Name;
    using ProfileNode = VARP.DataStructures.LinkedListNode<Profile>;
    using ProfileList = VARP.DataStructures.LinkedList<Profile>;


    public class Profile : ProfileNode, System.IDisposable
    {
        static Stopwatch SystemTime = Stopwatch.StartNew ( );

        public ProfileSample sample;
        public long startTime;

        public Profile ( ) : base(null)
        {
            Value = this;
        }

        public void Start(ProfileSample sample)
        {
            this.sample = sample;
            this.startTime = SystemTime.ElapsedMilliseconds;
        }

        public void Stop ( )
        {
            this.sample.UpdateTime ( SystemTime.ElapsedMilliseconds - this.startTime );
            this.startTime = 0;
            this.sample = null;
            this.Remove ( );
        }

        public void Cancel ( )
        {
            this.startTime = 0;
            this.sample = null;
            this.Remove ( );
        }

        public void Dispose ( )
        {
            this.Remove ( );
            this.Value = null;
        }
    }

    public class ProfilePool : System.IDisposable
    {
        ProfileList freeProfiles = new ProfileList ( );
        ProfileList usedProfiles = new ProfileList ( );

        public ProfilePool ( int size )
        {
            for ( var i = 0 ; i < size ; i++ )
                freeProfiles.AddFirst ( new Profile ( ) );
        }

        public Profile Start ( ProfileSample sample )
        {
            var profile = GetProfile ( );
            profile.Start ( sample );
            return profile;
        }

        private Profile GetProfile ()
        {
            var profile = freeProfiles.First.Value;
            if ( profile == null )
                profile = new Profile ( );
            usedProfiles.AddFirst( profile );
            UnityEngine.Debug.Assert ( Count < 1000 );
            return profile;
        }

        public void StopAllProfiles ( )
        {
            var curent = usedProfiles.First;
            while ( curent != null )
            {
                var next = curent.Next;
                curent.Value.Cancel ( );
                freeProfiles.AddFirst ( curent );
                curent = next;
            }
        }

        public int Count
        {
            get { return usedProfiles.Count; }
        }

        public void Dispose ( )
        {
            DisposeList ( freeProfiles.First );
            DisposeList ( usedProfiles.First );
        }

        private void DisposeList( ProfileNode node )
        {
            while ( node != null )
            {
                var next = node.Next;
                node.Value.Dispose ( );
                node = next;
            }
        }
    }

    public class ProfileSample : System.IDisposable
    {
        const int DEFAULT_HASH_SIZE = 10;

        public ProfileSample parent;
        public Name name;
        public int callsCount;
        public long elapseTime;
        public long elapseTimeMax;
        public System.Collections.Generic.Dictionary<Name, ProfileSample> children;

        public ProfileSample ( Name name )
        {
            this.name = name;
            this.parent = null;
            this.children = new Dictionary<Name, ProfileSample> ( DEFAULT_HASH_SIZE );
        }

        public ProfileSample ( Name name, ProfileSample parent )
        {
            this.name = name;
            this.parent = parent;
            children = new Dictionary<Name, ProfileSample> ( DEFAULT_HASH_SIZE );
        }

        public ProfileSample GetProfile ( Name name)
        {
            ProfileSample result = null;
            if ( children.TryGetValue ( name, out result ) )
                return result;
            children[ name ] = result = new ProfileSample ( name, this );
            return result;
        }

        public void UpdateTime( long elapseTime )
        {
            this.elapseTimeMax = System.Math.Max ( elapseTimeMax, elapseTime );
            this.elapseTime += elapseTime;
            this.callsCount++;
        }

        public void ClearTime()
        {
            this.elapseTime = 0;
            this.callsCount = 0;
            this.elapseTimeMax = 0;
        }

        public void Dispose ( )
        {
            parent = null;
            children.Clear ( );
        }
    }

    /**
     * Usage:
     * var fooProfile = ProfileManager.StartProfile(new Name("Foo"));
     * ....
     * var barProfile = ProfileManager.StartProfile(new Name("Bar"), new Name("Foo"));
     * ....
     * barProfile.Stop();
     * ....
     * fooProfile.Stop();
     * ....
     * ProfileManager.MakeTextReport();
     * ....
     * ProfileManager.ClearTime();
     */
    public static class ProfileManager 
    {
        public static void Init ( )
        {
            pofilePool.StopAllProfiles ( );
            Samples.Clear ( );
        }

        public static void PostInit ( )
        {
        }

        private static void DisposeAll()
        {
            pofilePool.Dispose ( );
            Samples.Clear ( );
        }

        public static void ClearTime()
        {
            foreach ( var sample in Samples)
                sample.Value.ClearTime ( );
        }

        private static ProfileSample GetProfile ( Name name )
        {
            ProfileSample sample = null;
            if ( Samples.TryGetValue ( name, out sample ) )
                return sample;
            sample = new ProfileSample ( name );
            Samples[ name ] = sample;
            return sample;
        }

        public static Profile StartProfile ( Name name )
        {
            ProfileSample sample = GetProfile( name );
            return pofilePool.Start ( sample );
        }

        public static Profile StartProfile ( Name name, Name parentName )
        {
            ProfileSample parentSample = GetProfile ( parentName );
            ProfileSample sample = parentSample.GetProfile ( name );
            return pofilePool.Start ( sample );
        }

        static ProfilePool pofilePool = new ProfilePool ( 128 );
        static Dictionary<Name, ProfileSample> Samples = new Dictionary<Name, ProfileSample> ( 128 );
    }
}