using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixIO.IO.Bmff
{
    public enum BoxTypes : uint
    {
        // Base Media File Format standard box types.

        ftyp = 0x66747970, // ISO/IEC 15596-12 4.3
        free = 0x66726565, // ISO/IEC 15596-12 8.1.2
        skip = 0x736b6970, // ISO/IEC 15596-12 8.1.2
        pdin = 0x7064696e, // ISO/IEC 15596-12 8.1.3
        moov = 0x6d6f6f76, // ISO/IEC 15596-12 8.2.1
        mvhd = 0x6d766864, // ISO/IEC 15596-12 8.2.2
        mdat = 0x6d646174, // ISO/IEC 15596-12 8.2.2
        trak = 0x7472616b, // ISO/IEC 15596-12 8.3.1
        tkhd = 0x746b6864, // ISO/IEC 15596-12 8.3.2
        tref = 0x74726566, // ISO/IEC 15596-12 8.3.3
        mdia = 0x6d646961, // ISO/IEC 15596-12 8.4
        mdhd = 0x6d646864, // ISO/IEC 15596-12 8.4.2
        hdlr = 0x68646c72, // ISO/IEC 15596-12 8.4.3
        minf = 0x6d696e66, // ISO/IEC 15596-12 8.4.4
        vmhd = 0x766d6864, // ISO/IEC 15596-12 8.4.5.2
        smhd = 0x736d6864, // ISO/IEC 15596-12 8.4.5.3
        hmhd = 0x686d6864, // ISO/IEC 15596-12 8.4.5.4
        nmhd = 0x6e6d6864, // ISO/IEC 15596-12 8.4.5.5
        dinf = 0x64696e66, // ISO/IEC 15596-12 8.5
        stbl = 0x7374626c, // ISO/IEC 15596-12 8.5
        stsd = 0x73747364, // ISO/IEC 15596-12 8.5.2
        stts = 0x73747473, // ISO/IEC 15596-12 8.6.1.2
        ctts = 0x63747473, // ISO/IEC 15596-12 8.6.1.3
        stss = 0x73747373, // ISO/IEC 15596-12 8.6.2
        stsh = 0x73747368, // ISO/IEC 15596-12 8.6.3
        edts = 0x65647473, // ISO/IEC 15596-12 8.6.4
        sdtp = 0x73647470, // ISO/IEC 15596-12 8.6.4
        elst = 0x656c7374, // ISO/IEC 15596-12 8.6.6
        dref = 0x64726566, // ISO/IEC 15596-12 8.7.2
        stsz = 0x7374737a, // ISO/IEC 15596-12 8.7.3.2
        stz2 = 0x73747a32, // ISO/IEC 15596-12 8.7.3.3
        stsc = 0x73747363, // ISO/IEC 15596-12 8.7.4
        stco = 0x7374636f, // ISO/IEC 15596-12 8.7.5
        co64 = 0x636f3634, // ISO/IEC 15596-12 8.7.5
        padb = 0x70616462, // ISO/IEC 15596-12 8.7.6
        stdp = 0x73746470, // ISO/IEC 15596-12 8.7.6
        subs = 0x73756273, // ISO/IEC 15596-12 8.7.7
        mvex = 0x6d766578, // ISO/IEC 15596-12 8.8.1
        mehd = 0x6d656864, // ISO/IEC 15596-12 8.8.2
        trex = 0x74726578, // ISO/IEC 15596-12 8.8.3
        moof = 0x6d6f6f66, // ISO/IEC 15596-12 8.8.4
        mfhd = 0x6d666864, // ISO/IEC 15596-12 8.8.5
        traf = 0x74726166, // ISO/IEC 15596-12 8.8.6
        tfhd = 0x74666864, // ISO/IEC 15596-12 8.8.7
        trun = 0x7472756e, // ISO/IEC 15596-12 8.8.8
        mfra = 0x6d667261, // ISO/IEC 15596-12 8.8.9
        tfra = 0x74667261, // ISO/IEC 15596-12 8.8.10
        mfro = 0x6d66726f, // ISO/IEC 15596-12 8.8.11
        sbgp = 0x73626770, // ISO/IEC 15596-12 8.9.2
        sgpd = 0x73677064, // ISO/IEC 15596-12 8.9.3
        udta = 0x75647461, // ISO/IEC 15596-12 8.10.1
        cprt = 0x63707274, // ISO/IEC 15596-12 8.10.2
        tsel = 0x7473656c, // ISO/IEC 15596-12 8.10.3
        meta = 0x6d657461, // ISO/IEC 15596-12 8.11.1
        xml  = 0x786d6c20, // ISO/IEC 15596-12 8.11.2
        bxml = 0x62786d6c, // ISO/IEC 15596-12 8.11.2
        iloc = 0x696c6f63, // ISO/IEC 15596-12 8.11.3
        pitm = 0x7069746d, // ISO/IEC 15596-12 8.11.4
        ipro = 0x6970726f, // ISO/IEC 15596-12 8.11.5
        iinf = 0x69696e66, // ISO/IEC 15596-12 8.11.6
        meco = 0x6d65636f, // ISO/IEC 15596-12 8.11.7
        mere = 0x6d657265, // ISO/IEC 15596-12 8.11.8
        sinf = 0x73696e66, // ISO/IEC 15596-12 8.12.1
        frma = 0x66726d61, // ISO/IEC 15596-12 8.12.2
        imif = 0x696d6966, // ISO/IEC 15596-12 8.12.3
        schm = 0x7363686d, // ISO/IEC 15596-12 8.12.4
        ipmc = 0x69706d63, // ISO/IEC 15596-12 8.12.4
        schi = 0x73636869, // ISO/IEC 15596-12 8.12.5
        fiin = 0x6669696e, // ISO/IEC 15596-12 8.13.2
        paen = 0x7061656e, // ISO/IEC 15596-12 8.13.2
        fpar = 0x66706172, // ISO/IEC 15596-12 8.13.3
        fecr = 0x66656372, // ISO/IEC 15596-12 8.13.4
        segr = 0x73656772, // ISO/IEC 15596-12 8.13.5
        gitn = 0x6769746e, // ISO/IEC 15596-12 8.13.6

        uuid = 0x75756964, // ISO/IEC 15596-12 11.1
    }
}
