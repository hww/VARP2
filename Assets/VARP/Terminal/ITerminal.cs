using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerminal
{
    // Write text to the input field
    void Write(string message);

    // Write text to the input field and add new line
    void WriteLine(string message);

    // Produce beep sound if it is defined
    void Beep();

    // Clear terminal screen
    void Clear();

    // Reset foreground color to defaut
    void ResetColor();

    // Set foreground color
    void SetColor(Color color);

    // Set background color
    void SetBackgroundColor ( Color color );

    // Set cursor position
    void SetCursor ( int x, int y );
}
