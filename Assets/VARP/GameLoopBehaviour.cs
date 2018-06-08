using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GameLoopBehaviour : MonoBehaviour {

    private void Start ( )
    {
        StartCoroutine ( GameLoop ( ) );   
    }

    public VARP.Player[] players = new VARP.Player[ 4 ];

    IEnumerator GameLoop()
    {
        SubSystems.Init ( );
        while (true)
        {

            yield return null;
            break;
        }
        SubSystems.DeInit ( );
    }


}

namespace VARP
{
    using DataStructures;

    /**
     * Game object
     * Used for all actors in the game
     */
    public class GObject : LinkedListNode<GObject>
    {
        public GObject self;
        public GObject parent;
        public GObject sibling;
        public GObject creator;
        public GObject player;
        public GObject collider;
        public GObject interrupter;
        public LinkedList<GObject> children = new LinkedList<GObject> ( );

        public GObject ( ) : base ( null )
        {
            this.Value = this;
            this.self = this;
        }

        public GObject ( int memorySize ) : base ( null )
        {
            this.Value = this;
            this.self = this;
        }

        public void Create ( GObject parent, int type, int subtype, int flags, params Variant[] arguments)
        {

        }

        public void Init ( int type, int subtype, int flags, params Variant[] arguments )
        {

        }

    }

    /** The pointer to the object by number of the objects in memory */
    public struct GObjectHandle
    {
        public GObjectHandle(int idx)
        {
            index = idx;
        }

        public GObjectHandle( GObjectHandle handle )
        {
            index = handle.index;
        }

        public GObject Link
        {
            get { return GObjectPool.GetItme ( index ); }
        }

        public static implicit operator GObject ( GObjectHandle handle )
        {
            return GObjectPool.GetItme ( handle.index );
        }

        public static implicit operator GObjectHandle ( int index )
        {
            return new GObjectHandle( index ) ;
        }

        public override string ToString ( )
        {
            return string.Format ( "<GObject #{0}>", index );
        }

        public int index;
    }

    public class GObjectPool 
    {
        static GObject[] allObjects;
        static DataStructures.LinkedList<GObject> freeObjects = new DataStructures.LinkedList<GObject> ( );

        public static void Init(int capactity, int memorySize )
        {
            allObjects = new GObject[ capactity ];
            for ( var i = 0 ; i < capactity ; i++ )
                freeObjects.AddFirst ( new GObject ( memorySize ) );
        }

        public static void DeInit (  )
        {
            allObjects = null;
            freeObjects.Clear();
        }

        public static GObject GetFree()
        {
            var obj = freeObjects.First;
            if ( obj == null )
                throw new System.OverflowException ( );
            obj.Remove ( );
            return obj.Value;
        }

        public static void ReleaseChildren ( GObject gobject )
        {
            var curent = gobject.children.First;
            while ( curent != null )
            {
                var next = curent.Next;
                Release ( curent.Value );
                curent = next;
            }
            gobject.children.Clear ( );
        }

        public static void Release(GObject gobject)
        {
            ReleaseChildren ( gobject );
            gobject.parent = null;
            gobject.sibling = null;
            gobject.creator = null;
            gobject.player = null;
            gobject.collider = null;
            gobject.interrupter = null;
            freeObjects.AddFirst ( gobject );
        }

        public static void Release (GObjectHandle handle)
        {
            Release ( handle.Link );
        }

        public static GObject GetItme( int key )
        {
            Debug.Assert ( key < allObjects.Length );
            return allObjects [ key ];
        }
    }



    /**
     * Player
     * Has it's own unique fields from 
     */
    public class Player : GObject
    {
        public Player ( ) : base ( 2048 )
        {

        }
    }

    /**
     * Stack frame
     */
    public struct Stack
    {
        public Variant[] memory;
        public Stack ( int capacity )
        {
            memory = new Variant[ capacity ];
        }
    }

    /** The code block for virtual machine */
    public class ByteCode
    {

    }

    public class VM
    {
        public delegate void OpcodeImplementation ( );
        public static OpcodeImplementation[] oprcodes = new OpcodeImplementation[]
        {

        };

        public static void ExecuteCode ( ByteCode byteCode )
        {

        }
    }

    public unsafe struct Instruction
    {
        public Instruction ( Opcode opcode, byte a, byte b, byte c)
        {
            instruction = (uint)(((byte)opcode << 24) & (a << 16) & (b << 8) & c);
        }

        public Instruction ( Opcode opcode, short ax, short bx )
        {
            instruction = (uint)( ( (byte)opcode << 24 ) & ( a << 12 ) & b );
        }

        public Opcode opcode
        {
            get { return (Opcode)(instruction >> 24); }
        }
        public byte a
        {
            get { return (byte)(( instruction >> 16 ) & 0xFF); }
        }
        public byte b
        {
            get { return (byte)( ( instruction >> 16 ) & 0xFF ); }
        }
        public byte c
        {
            get { return (byte)( instruction & 0xFF ); }
        }
        public short ax
        {
            get { return (byte)( ( instruction >> 12 ) & 0xFFF ); }
        }
        public short bx
        {
            get { return (byte)( instruction& 0xFFF ); }
        }
        public int axx
        {
            get { return (byte)( instruction & 0xFFFFFF ); }
        }
        public uint instruction;
    }

    public enum Opcode
    {
    }

    public struct StateDesscriptor
    {

    }

    public class Executable
    {
        public Instruction[] instructions;
        public Variant[] constants;

        public Executable(Instruction[] instructions, Variant[] constants)
        {
            this.instructions = instructions;
            this.constants = constants;
        }
    }

    public class ExecutableBuider
    {
        public List<Instruction> instructions;
        public List<Variant> constants;

        public ExecutableBuider()
        {
            instructions = new List<Instruction> ( 100 );
            constants = new List<Variant> ( 100 );
        }

        public ExecutableBuider ( int capacity )
        {
            instructions = new List<Instruction> ( capacity );
            constants = new List<Variant> ( capacity );
        }

        public void Clear()
        {
            instructions.Clear ( );
            constants.Clear ( );
        }

        public int Add(Instruction instruction)
        {
            instructions.Add ( instruction );
            return instructions.Count - 1;
        }

        public int Add ( Variant constant )
        {
            constants.Add ( constant );
            return constants.Count - 1;
        }

        public Executable AsExecutable
        {
            get { return new Executable ( instructions.ToArray ( ), constants.ToArray ( )); }
        }
    }

}