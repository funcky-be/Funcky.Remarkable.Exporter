// -----------------------------------------------------------------------
//  <copyright file="Layer.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Layer
    {
        public List<Stroke> Strokes { get; } = new List<Stroke>();
    }
}