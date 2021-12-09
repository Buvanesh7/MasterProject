using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
    public class BoundingBoxHelper
    {
        double xmin, ymin, zmin, xmax, ymax, zmax;
        public BoundingBoxHelper()
        {
            xmin = ymin = zmin = double.MaxValue;
            xmax = ymax = zmax = double.MinValue;
        }

        public XYZ Min
        {
            get { return new XYZ(xmin, ymin, zmin); }
        }

        public XYZ Max
        {
            get { return new XYZ(xmax, ymax, zmax); }
        }

        public XYZ MidPoint
        {
            get { return 0.5 * (Min + Max); }
        }

        public void ExpandToContain(XYZ p)
        {
            if (p.X < xmin) { xmin = p.X; }
            if (p.Y < ymin) { ymin = p.Y; }
            if (p.Z < zmin) { zmin = p.Z; }
            if (p.X > xmax) { xmax = p.X; }
            if (p.Y > ymax) { ymax = p.Y; }
            if (p.Z > zmax) { zmax = p.Z; }
        }
    }
}
