using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSMapFile
{
    class GSA
    {
//        $GPGSA,A,3,04,05,,09,12,,,24,,,,,2.5,1.3,2.1*39

//Where:
//GSA Satellite status
//A Auto selection of 2D or 3D fix (M = manual)
//3 3D fix - values include: 1 = no fix
//                           2 = 2D fix
//                           3 = 3D fix
//04,05... PRNs of satellites used for fix (space for 12)
//2.5 PDOP (dilution of precision)
//1.3 Horizontal dilution of precision (HDOP)
//2.1 Vertical dilution of precision (VDOP)
//*39 the checksum data, always begins with *


        public string Name;
        public string SelectionMode;
        public int ThreeDFix;
        public double[] PRNsOfSatellites= new double[12];
        public double PDOP;
        public double HDOP;
        public double VDOP;
        public string Checksum;
    }
}
