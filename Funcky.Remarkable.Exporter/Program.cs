// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Remarkable.Exporter.Workers;

    using NLog;

    internal static class Program
    {
        public static async Task Main()
        {
            await SynchronizeNotes.Execute();

            ExtractNotes.Execute();

            DrawNotes.Execute();

            SaveToEvernote.Execute();

            LogManager.Shutdown();
        }
    }
}