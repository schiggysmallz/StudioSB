﻿using StudioSB.Tools;

namespace StudioSB.Scenes.LVD
{
    public class LVDBounds : LVDEntry
    {
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

        public override void Read(BinaryReaderExt r)
        {
            base.Read(r);

            r.Skip(1);
            Left = r.ReadSingle();
            Right = r.ReadSingle();
            Top = r.ReadSingle();
            Bottom = r.ReadSingle();
        }

        public override void Write(BinaryWriterExt writer)
        {
            base.Write(writer);

            writer.Write((byte)1);
            writer.Write(Left);
            writer.Write(Right);
            writer.Write(Top);
            writer.Write(Bottom);
        }
    }
}
