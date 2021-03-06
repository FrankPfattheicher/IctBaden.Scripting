﻿using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace IctBaden.Scripting
{
    public class ScriptError : EventArgs
    {
        public int Line { get; private set; }
        public int Column { get; private set; }
        public string Message { get; private set; }

        public ScriptError(int line, int column, string message)
        {
            Line = line;
            Column = column;
            Message = message;
        }
    }
}
