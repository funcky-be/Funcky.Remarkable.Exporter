// -----------------------------------------------------------------------
//  <copyright file="Stroke.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System.Collections.Generic;

    public class Stroke
    {
        public int PenType { get; set; }

        public int PenColor { get; set; }

        public int Padding { get; set; }

        public float PenWidth { get; set; }

        public List<Segment> Segments { get; } = new List<Segment>();
    }
}