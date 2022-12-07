using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SoulsFormats;

namespace FXRTestBed
{
    /// <summary>
    /// An SFX definition file used in DS3 and Sekiro. Extension: .fxr
    /// </summary>
    public class Fxr3Hoodie : SoulsFile<Fxr3Hoodie>
    {
        public FxrVersion Version { get; set; }

        public int Id { get; set; }

        public FfxStateMachine RootStateMachine { get; set; }

        public FfxEffectCallA RootEffectCall { get; set; }

        public List<int> Section12S { get; set; }

        public List<int> Section13S { get; set; }

        public Fxr3Hoodie()
        {
            Version = FxrVersion.DarkSouls3;
            RootStateMachine = new FfxStateMachine();
            RootEffectCall = new FfxEffectCallA();
            Section12S = new List<int>();
            Section13S = new List<int>();
        }

        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 8L)
                return false;
            string ascii = br.GetASCII(0L, 4);
            short int16 = br.GetInt16(6L);
            return ascii == "FXR\0" && (int16 == 4 || int16 == 5);
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("FXR\0");
            int num1 = br.AssertInt16(new short[1]);
            Version = br.ReadEnum16<FxrVersion>();
            br.AssertInt32(1);
            Id = br.ReadInt32();
            int num2 = br.ReadInt32();
            br.AssertInt32(1);
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            int num3 = br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.AssertInt32(1);
            br.AssertInt32(new int[1]);
            if (Version == FxrVersion.Sekiro)
            {
                int num4 = br.ReadInt32();
                int count1 = br.ReadInt32();
                int num5 = br.ReadInt32();
                int count2 = br.ReadInt32();
                br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                Section12S = new List<int>(br.GetInt32s(num4, count1));
                Section13S = new List<int>(br.GetInt32s(num5, count2));
            }
            else
            {
                Section12S = new List<int>();
                Section13S = new List<int>();
            }
            br.Position = num2;
            RootStateMachine = new FfxStateMachine(br);
            br.Position = num3;
            RootEffectCall = new FfxEffectCallA(br);
        }

        protected override void Write(BinaryWriterEx bw)
        {
            bw.WriteASCII("FXR\0");
            bw.WriteInt16(0);
            bw.WriteUInt16((ushort)Version);
            bw.WriteInt32(1);
            bw.WriteInt32(Id);
            bw.ReserveInt32("Section1Offset");
            bw.WriteInt32(1);
            bw.ReserveInt32("Section2Offset");
            bw.WriteInt32(RootStateMachine.States.Count);
            bw.ReserveInt32("Section3Offset");
            bw.ReserveInt32("Section3Count");
            bw.ReserveInt32("Section4Offset");
            bw.ReserveInt32("Section4Count");
            bw.ReserveInt32("Section5Offset");
            bw.ReserveInt32("Section5Count");
            bw.ReserveInt32("Section6Offset");
            bw.ReserveInt32("Section6Count");
            bw.ReserveInt32("Section7Offset");
            bw.ReserveInt32("Section7Count");
            bw.ReserveInt32("Section8Offset");
            bw.ReserveInt32("Section8Count");
            bw.ReserveInt32("Section9Offset");
            bw.ReserveInt32("Section9Count");
            bw.ReserveInt32("Section10Offset");
            bw.ReserveInt32("Section10Count");
            bw.ReserveInt32("Section11Offset");
            bw.ReserveInt32("Section11Count");
            bw.WriteInt32(1);
            bw.WriteInt32(0);
            if (Version == FxrVersion.Sekiro)
            {
                bw.ReserveInt32("Section12Offset");
                bw.WriteInt32(Section12S.Count);
                bw.ReserveInt32("Section13Offset");
                bw.WriteInt32(Section13S.Count);
                bw.ReserveInt32("Section14Offset");
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
            bw.FillInt32("Section1Offset", (int)bw.Position);
            RootStateMachine.Write(bw);
            bw.Pad(16);
            bw.FillInt32("Section2Offset", (int)bw.Position);
            RootStateMachine.WriteSection2S(bw);
            bw.Pad(16);
            bw.FillInt32("Section3Offset", (int)bw.Position);
            List<FfxState> states = RootStateMachine.States;
            List<FfxTransition> section3S = new List<FfxTransition>();
            for (int index = 0; index < states.Count; ++index)
                states[index].WriteSection3S(bw, index, section3S);
            bw.FillInt32("Section3Count", section3S.Count);
            bw.Pad(16);
            bw.FillInt32("Section4Offset", (int)bw.Position);
            List<FfxEffectCallA> section4S = new List<FfxEffectCallA>();
            RootEffectCall.Write(bw, section4S);
            RootEffectCall.WriteSection4S(bw, section4S);
            bw.FillInt32("Section4Count", section4S.Count);
            bw.Pad(16);
            bw.FillInt32("Section5Offset", (int)bw.Position);
            int section5Count = 0;
            for (int index = 0; index < section4S.Count; ++index)
                section4S[index].WriteSection5S(bw, index, ref section5Count);
            bw.FillInt32("Section5Count", section5Count);
            bw.Pad(16);
            bw.FillInt32("Section6Offset", (int)bw.Position);
            section5Count = 0;
            List<FfxActionCall> section6S = new List<FfxActionCall>();
            for (int index = 0; index < section4S.Count; ++index)
                section4S[index].WriteSection6S(bw, index, ref section5Count, section6S);
            bw.FillInt32("Section6Count", section6S.Count);
            bw.Pad(16);
            bw.FillInt32("Section7Offset", (int)bw.Position);
            List<FfxProperty> section7S = new List<FfxProperty>();
            for (int index = 0; index < section6S.Count; ++index)
                section6S[index].WriteSection7S(bw, index, section7S);
            bw.FillInt32("Section7Count", section7S.Count);
            bw.Pad(16);
            bw.FillInt32("Section8Offset", (int)bw.Position);
            List<Section8> section8S = new List<Section8>();
            for (int index = 0; index < section7S.Count; ++index)
                section7S[index].WriteSection8S(bw, index, section8S);
            bw.FillInt32("Section8Count", section8S.Count);
            bw.Pad(16);
            bw.FillInt32("Section9Offset", (int)bw.Position);
            List<Section9> section9S = new List<Section9>();
            for (int index = 0; index < section8S.Count; ++index)
                section8S[index].WriteSection9S(bw, index, section9S);
            bw.FillInt32("Section9Count", section9S.Count);
            bw.Pad(16);
            bw.FillInt32("Section10Offset", (int)bw.Position);
            List<Section10> section10S = new List<Section10>();
            for (int index = 0; index < section6S.Count; ++index)
                section6S[index].WriteSection10S(bw, index, section10S);
            bw.FillInt32("Section10Count", section10S.Count);
            bw.Pad(16);
            bw.FillInt32("Section11Offset", (int)bw.Position);
            int section11Count = 0;
            for (int index = 0; index < section3S.Count; ++index)
                section3S[index].WriteSection11S(bw, index, ref section11Count);
            for (int index = 0; index < section6S.Count; ++index)
                section6S[index].WriteSection11S(bw, index, ref section11Count);
            for (int index = 0; index < section7S.Count; ++index)
                section7S[index].WriteSection11S(bw, index, ref section11Count);
            for (int index = 0; index < section8S.Count; ++index)
                section8S[index].WriteSection11S(bw, index, ref section11Count);
            for (int index = 0; index < section9S.Count; ++index)
                section9S[index].WriteSection11S(bw, index, ref section11Count);
            for (int index = 0; index < section10S.Count; ++index)
                section10S[index].WriteSection11S(bw, index, ref section11Count);
            bw.FillInt32("Section11Count", section11Count);
            bw.Pad(16);
            if (Version != FxrVersion.Sekiro)
                return;
            bw.FillInt32("Section12Offset", (int)bw.Position);
            bw.WriteInt32s(Section12S);
            bw.Pad(16);
            bw.FillInt32("Section13Offset", (int)bw.Position);
            bw.WriteInt32s(Section13S);
            bw.Pad(16);
            bw.FillInt32("Section14Offset", (int)bw.Position);
        }

        public enum FxrVersion : ushort
        {
            DarkSouls3 = 4,
            Sekiro = 5,
        }

        public class FfxStateMachine
        {
            public List<FfxState> States { get; set; }

            public FfxStateMachine() => States = new List<FfxState>();

            internal FfxStateMachine(BinaryReaderEx br)
            {
                br.AssertInt32(new int[1]);
                int capacity = br.ReadInt32();
                int num = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num);
                States = new List<FfxState>(capacity);
                for (int index = 0; index < capacity; ++index)
                    States.Add(new FfxState(br));
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteInt32(0);
                bw.WriteInt32(States.Count);
                bw.ReserveInt32("Section1Section2sOffset");
                bw.WriteInt32(0);
            }

            internal void WriteSection2S(BinaryWriterEx bw)
            {
                bw.FillInt32("Section1Section2sOffset", (int)bw.Position);
                for (int index = 0; index < States.Count; ++index)
                    States[index].Write(bw, index);
            }
        }

        public class FfxState
        {
            public List<FfxTransition> Transitions { get; set; }

            public FfxState() => Transitions = new List<FfxTransition>();

            internal FfxState(BinaryReaderEx br)
            {
                br.AssertInt32(new int[1]);
                int capacity = br.ReadInt32();
                int num = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num);
                Transitions = new List<FfxTransition>(capacity);
                for (int index = 0; index < capacity; ++index)
                    Transitions.Add(new FfxTransition(br));
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(0);
                bw.WriteInt32(Transitions.Count);
                bw.ReserveInt32(string.Format("Section2Section3sOffset[{0}]", index));
                bw.WriteInt32(0);
            }

            internal void WriteSection3S(
              BinaryWriterEx bw,
              int index,
              List<FfxTransition> section3S)
            {
                bw.FillInt32(string.Format("Section2Section3sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxTransition transition in Transitions)
                    transition.Write(bw, section3S);
            }
        }

        public class FfxTransition
        {
            [XmlAttribute]
            public int TargetStateIndex { get; set; }

            public int Unk10 { get; set; }

            public int Unk38 { get; set; }

            public int Section11Data1 { get; set; }

            public int Section11Data2 { get; set; }

            public FfxTransition()
            {
            }

            internal FfxTransition(BinaryReaderEx br)
            {
                int num1 = br.AssertInt16(11);
                int num2 = br.AssertByte(new byte[1]);
                int num3 = br.AssertByte(1);
                br.AssertInt32(new int[1]);
                TargetStateIndex = br.ReadInt32();
                br.AssertInt32(new int[1]);
                Unk10 = br.AssertInt32(16842748, 16842749);
                br.AssertInt32(new int[1]);
                br.AssertInt32(1);
                br.AssertInt32(new int[1]);
                int num4 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                Unk38 = br.AssertInt32(16842748, 16842749);
                br.AssertInt32(new int[1]);
                br.AssertInt32(1);
                br.AssertInt32(new int[1]);
                int num5 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                Section11Data1 = br.GetInt32(num4);
                Section11Data2 = br.GetInt32(num5);
            }

            internal void Write(BinaryWriterEx bw, List<FfxTransition> section3S)
            {
                int count = section3S.Count;
                bw.WriteInt16(11);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(0);
                bw.WriteInt32(TargetStateIndex);
                bw.WriteInt32(0);
                bw.WriteInt32(Unk10);
                bw.WriteInt32(0);
                bw.WriteInt32(1);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section3Section11Offset1[{0}]", count));
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(Unk38);
                bw.WriteInt32(0);
                bw.WriteInt32(1);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section3Section11Offset2[{0}]", count));
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                section3S.Add(this);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                bw.FillInt32(string.Format("Section3Section11Offset1[{0}]", index), (int)bw.Position);
                bw.WriteInt32(Section11Data1);
                bw.FillInt32(string.Format("Section3Section11Offset2[{0}]", index), (int)bw.Position);
                bw.WriteInt32(Section11Data2);
                section11Count += 2;
            }
        }

        public class FfxEffectCallA
        {
            [XmlAttribute]
            public short EffectId { get; set; }

            public List<FfxEffectCallA> EffectAs { get; set; }

            public List<FfxEffectCallB> EffectBs { get; set; }

            public List<FfxActionCall> Actions { get; set; }

            public FfxEffectCallA()
            {
                EffectAs = new List<FfxEffectCallA>();
                EffectBs = new List<FfxEffectCallB>();
                Actions = new List<FfxActionCall>();
            }

            internal FfxEffectCallA(BinaryReaderEx br)
            {
                EffectId = br.ReadInt16();
                int num1 = br.AssertByte(new byte[1]);
                int num2 = br.AssertByte(1);
                br.AssertInt32(new int[1]);
                int capacity1 = br.ReadInt32();
                int capacity2 = br.ReadInt32();
                int capacity3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num4 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num5 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num5);
                EffectAs = new List<FfxEffectCallA>(capacity3);
                for (int index = 0; index < capacity3; ++index)
                    EffectAs.Add(new FfxEffectCallA(br));
                br.StepOut();
                br.StepIn(num3);
                EffectBs = new List<FfxEffectCallB>(capacity1);
                for (int index = 0; index < capacity1; ++index)
                    EffectBs.Add(new FfxEffectCallB(br));
                br.StepOut();
                br.StepIn(num4);
                Actions = new List<FfxActionCall>(capacity2);
                for (int index = 0; index < capacity2; ++index)
                    Actions.Add(new FfxActionCall(br));
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, List<FfxEffectCallA> section4S)
            {
                int count = section4S.Count;
                bw.WriteInt16(EffectId);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(0);
                bw.WriteInt32(EffectBs.Count);
                bw.WriteInt32(Actions.Count);
                bw.WriteInt32(EffectAs.Count);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section4Section5sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section4Section6sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section4Section4sOffset[{0}]", count));
                bw.WriteInt32(0);
                section4S.Add(this);
            }

            internal void WriteSection4S(BinaryWriterEx bw, List<FfxEffectCallA> section4S)
            {
                int num = section4S.IndexOf(this);
                if (EffectAs.Count == 0)
                {
                    bw.FillInt32(string.Format("Section4Section4sOffset[{0}]", num), 0);
                }
                else
                {
                    bw.FillInt32(string.Format("Section4Section4sOffset[{0}]", num), (int)bw.Position);
                    foreach (FfxEffectCallA effectA in EffectAs)
                        effectA.Write(bw, section4S);
                    foreach (FfxEffectCallA effectA in EffectAs)
                        effectA.WriteSection4S(bw, section4S);
                }
            }

            internal void WriteSection5S(BinaryWriterEx bw, int index, ref int section5Count)
            {
                if (EffectBs.Count == 0)
                {
                    bw.FillInt32(string.Format("Section4Section5sOffset[{0}]", index), 0);
                }
                else
                {
                    bw.FillInt32(string.Format("Section4Section5sOffset[{0}]", index), (int)bw.Position);
                    for (int index1 = 0; index1 < EffectBs.Count; ++index1)
                        EffectBs[index1].Write(bw, section5Count + index1);
                    section5Count += EffectBs.Count;
                }
            }

            internal void WriteSection6S(
              BinaryWriterEx bw,
              int index,
              ref int section5Count,
              List<FfxActionCall> section6S)
            {
                bw.FillInt32(string.Format("Section4Section6sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxActionCall action in Actions)
                    action.Write(bw, section6S);
                for (int index1 = 0; index1 < EffectBs.Count; ++index1)
                    EffectBs[index1].WriteSection6S(bw, section5Count + index1, section6S);
                section5Count += EffectBs.Count;
            }
        }

        public class FfxEffectCallB
        {
            [XmlAttribute]
            public short EffectId { get; set; }

            public List<FfxActionCall> Actions { get; set; }

            public FfxEffectCallB() => Actions = new List<FfxActionCall>();

            internal FfxEffectCallB(BinaryReaderEx br)
            {
                EffectId = br.ReadInt16();
                int num1 = br.AssertByte(new byte[1]);
                int num2 = br.AssertByte(1);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                int capacity = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                int num3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num3);
                Actions = new List<FfxActionCall>(capacity);
                for (int index = 0; index < capacity; ++index)
                    Actions.Add(new FfxActionCall(br));
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt16(EffectId);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(Actions.Count);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section5Section6sOffset[{0}]", index));
                bw.WriteInt32(0);
            }

            internal void WriteSection6S(
              BinaryWriterEx bw,
              int index,
              List<FfxActionCall> section6S)
            {
                bw.FillInt32(string.Format("Section5Section6sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxActionCall action in Actions)
                    action.Write(bw, section6S);
            }
        }

        public class FfxActionCall
        {
            [XmlAttribute]
            public short ActionId { get; set; }

            public bool Unk02 { get; set; }

            public bool Unk03 { get; set; }

            public int Unk04 { get; set; }

            public List<FfxProperty> Properties1 { get; set; }

            public List<FfxProperty> Properties2 { get; set; }

            public List<Section10> Section10S { get; set; }

            public List<FfxField> Fields1 { get; set; }

            public List<FfxField> Fields2 { get; set; }

            public FfxActionCall()
            {
                Properties1 = new List<FfxProperty>();
                Properties2 = new List<FfxProperty>();
                Section10S = new List<Section10>();
                Fields1 = new List<FfxField>();
                Fields2 = new List<FfxField>();
            }

            internal FfxActionCall(BinaryReaderEx br)
            {
                ActionId = br.ReadInt16();
                Unk02 = br.ReadBoolean();
                Unk03 = br.ReadBoolean();
                Unk04 = br.ReadInt32();
                int count1 = br.ReadInt32();
                int capacity1 = br.ReadInt32();
                int capacity2 = br.ReadInt32();
                int count2 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int capacity3 = br.ReadInt32();
                int num1 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num2 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.AssertInt32(new int[1]);
                br.StepIn(num3);
                Properties1 = new List<FfxProperty>(capacity2);
                for (int index = 0; index < capacity2; ++index)
                    Properties1.Add(new FfxProperty(br));
                Properties2 = new List<FfxProperty>(capacity3);
                for (int index = 0; index < capacity3; ++index)
                    Properties2.Add(new FfxProperty(br));
                br.StepOut();
                br.StepIn(num2);
                Section10S = new List<Section10>(capacity1);
                for (int index = 0; index < capacity1; ++index)
                    Section10S.Add(new Section10(br));
                br.StepOut();
                br.StepIn(num1);
                Fields1 = FfxField.ReadMany(br, count1);
                Fields2 = FfxField.ReadMany(br, count2);
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, List<FfxActionCall> section6S)
            {
                int count = section6S.Count;
                bw.WriteInt16(ActionId);
                bw.WriteBoolean(Unk02);
                bw.WriteBoolean(Unk03);
                bw.WriteInt32(Unk04);
                bw.WriteInt32(Fields1.Count);
                bw.WriteInt32(Section10S.Count);
                bw.WriteInt32(Properties1.Count);
                bw.WriteInt32(Fields2.Count);
                bw.WriteInt32(0);
                bw.WriteInt32(Properties2.Count);
                bw.ReserveInt32(string.Format("Section6Section11sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section6Section10sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section6Section7sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                section6S.Add(this);
            }

            internal void WriteSection7S(BinaryWriterEx bw, int index, List<FfxProperty> section7S)
            {
                bw.FillInt32(string.Format("Section6Section7sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxProperty ffxProperty in Properties1)
                    ffxProperty.Write(bw, section7S);
                foreach (FfxProperty ffxProperty in Properties2)
                    ffxProperty.Write(bw, section7S);
            }

            internal void WriteSection10S(BinaryWriterEx bw, int index, List<Section10> section10S)
            {
                bw.FillInt32(string.Format("Section6Section10sOffset[{0}]", index), (int)bw.Position);
                foreach (Section10 section10 in Section10S)
                    section10.Write(bw, section10S);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                if (Fields1.Count == 0 && Fields2.Count == 0)
                {
                    bw.FillInt32(string.Format("Section6Section11sOffset[{0}]", index), 0);
                }
                else
                {
                    bw.FillInt32(string.Format("Section6Section11sOffset[{0}]", index), (int)bw.Position);
                    foreach (FfxField ffxField in Fields1)
                        ffxField.Write(bw);
                    foreach (FfxField ffxField in Fields2)
                        ffxField.Write(bw);
                    section11Count += Fields1.Count + Fields2.Count;
                }
            }
        }

        [XmlInclude(typeof(FfxFieldFloat))]
        [XmlInclude(typeof(FfxFieldInt))]
        public abstract class FfxField
        {
            public static FfxField Read(BinaryReaderEx br)
            {
                float single = br.GetSingle(br.Position);
                FfxField ffxField;
                if (single >= 9.99999974737875E-05 && single < 1000000.0 || single <= -9.99999974737875E-05 && single > -1000000.0)
                    ffxField = new FfxFieldFloat
                    {
                        Value = single
                    };
                else
                    ffxField = new FfxFieldInt
                    {
                        Value = br.GetInt32(br.Position)
                    };
                br.Position += 4L;
                return ffxField;
            }

            public static List<FfxField> ReadMany(BinaryReaderEx br, int count)
            {
                List<FfxField> ffxFieldList = new List<FfxField>();
                for (int index = 0; index < count; ++index)
                    ffxFieldList.Add(Read(br));
                return ffxFieldList;
            }

            public static List<FfxField> ReadManyAt(
              BinaryReaderEx br,
              int offset,
              int count)
            {
                br.StepIn(offset);
                List<FfxField> ffxFieldList = ReadMany(br, count);
                br.StepOut();
                return ffxFieldList;
            }

            public abstract void Write(BinaryWriterEx bw);

            public class FfxFieldFloat : FfxField
            {
                [XmlAttribute]
                public float Value;

                public override void Write(BinaryWriterEx bw) => bw.WriteSingle(Value);
            }

            public class FfxFieldInt : FfxField
            {
                [XmlAttribute]
                public int Value;

                public override void Write(BinaryWriterEx bw) => bw.WriteInt32(Value);
            }
        }

        public class FfxProperty
        {
            [XmlAttribute]
            public short TypeEnumA { get; set; }

            [XmlAttribute]
            public int TypeEnumB { get; set; }

            public List<Section8> Section8S { get; set; }

            public List<FfxField> Fields { get; set; }

            public FfxProperty()
            {
                Section8S = new List<Section8>();
                Fields = new List<FfxField>();
            }

            internal FfxProperty(BinaryReaderEx br)
            {
                TypeEnumA = br.ReadInt16();
                int num1 = br.AssertByte(new byte[1]);
                int num2 = br.AssertByte(1);
                TypeEnumB = br.ReadInt32();
                int count = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int offset = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int capacity = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num3);
                Section8S = new List<Section8>(capacity);
                for (int index = 0; index < capacity; ++index)
                    Section8S.Add(new Section8(br));
                br.StepOut();
                Fields = new List<FfxField>();
                Fields = FfxField.ReadManyAt(br, offset, count);
            }

            internal void Write(BinaryWriterEx bw, List<FfxProperty> section7S)
            {
                int count = section7S.Count;
                bw.WriteInt16(TypeEnumA);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(TypeEnumB);
                bw.WriteInt32(Fields.Count);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section7Section11sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section7Section8sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.WriteInt32(Section8S.Count);
                bw.WriteInt32(0);
                section7S.Add(this);
            }

            internal void WriteSection8S(BinaryWriterEx bw, int index, List<Section8> section8S)
            {
                bw.FillInt32(string.Format("Section7Section8sOffset[{0}]", index), (int)bw.Position);
                foreach (Section8 section8 in Section8S)
                    section8.Write(bw, section8S);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                if (Fields.Count == 0)
                {
                    bw.FillInt32(string.Format("Section7Section11sOffset[{0}]", index), 0);
                }
                else
                {
                    bw.FillInt32(string.Format("Section7Section11sOffset[{0}]", index), (int)bw.Position);
                    foreach (FfxField field in Fields)
                        field.Write(bw);
                    section11Count += Fields.Count;
                }
            }
        }

        public class Section8
        {
            [XmlAttribute]
            public ushort Unk00 { get; set; }

            public int Unk04 { get; set; }

            public List<Section9> Section9S { get; set; }

            public List<FfxField> Fields { get; set; }

            public Section8()
            {
                Section9S = new List<Section9>();
                Fields = new List<FfxField>();
            }

            internal Section8(BinaryReaderEx br)
            {
                Unk00 = br.ReadUInt16();
                int num1 = br.AssertByte(new byte[1]);
                int num2 = br.AssertByte(1);
                Unk04 = br.ReadInt32();
                int count = br.ReadInt32();
                int capacity = br.ReadInt32();
                int offset = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int num3 = br.ReadInt32();
                br.AssertInt32(new int[1]);
                br.StepIn(num3);
                Section9S = new List<Section9>(capacity);
                for (int index = 0; index < capacity; ++index)
                    Section9S.Add(new Section9(br));
                br.StepOut();
                Fields = FfxField.ReadManyAt(br, offset, count);
            }

            internal void Write(BinaryWriterEx bw, List<Section8> section8S)
            {
                int count = section8S.Count;
                bw.WriteUInt16(Unk00);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(Unk04);
                bw.WriteInt32(Fields.Count);
                bw.WriteInt32(Section9S.Count);
                bw.ReserveInt32(string.Format("Section8Section11sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section8Section9sOffset[{0}]", count));
                bw.WriteInt32(0);
                section8S.Add(this);
            }

            internal void WriteSection9S(BinaryWriterEx bw, int index, List<Section9> section9S)
            {
                bw.FillInt32(string.Format("Section8Section9sOffset[{0}]", index), (int)bw.Position);
                foreach (Section9 section9 in Section9S)
                    section9.Write(bw, section9S);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                bw.FillInt32(string.Format("Section8Section11sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxField field in Fields)
                    field.Write(bw);
                section11Count += Fields.Count;
            }
        }

        public class Section9
        {
            public int Unk04 { get; set; }

            public List<FfxField> Fields { get; set; }

            public Section9() => Fields = new List<FfxField>();

            internal Section9(BinaryReaderEx br)
            {
                int num1 = br.AssertInt16(48);
                int num2 = br.AssertByte(new byte[1]);
                int num3 = br.AssertByte(1);
                Unk04 = br.ReadInt32();
                int count = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int offset = br.ReadInt32();
                br.AssertInt32(new int[1]);
                Fields = FfxField.ReadManyAt(br, offset, count);
            }

            internal void Write(BinaryWriterEx bw, List<Section9> section9S)
            {
                int count = section9S.Count;
                bw.WriteInt16(48);
                bw.WriteByte(0);
                bw.WriteByte(1);
                bw.WriteInt32(Unk04);
                bw.WriteInt32(Fields.Count);
                bw.WriteInt32(0);
                bw.ReserveInt32(string.Format("Section9Section11sOffset[{0}]", count));
                bw.WriteInt32(0);
                section9S.Add(this);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                bw.FillInt32(string.Format("Section9Section11sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxField field in Fields)
                    field.Write(bw);
                section11Count += Fields.Count;
            }
        }

        public class Section10
        {
            public List<FfxField> Fields { get; set; }

            public Section10() => Fields = new List<FfxField>();

            internal Section10(BinaryReaderEx br)
            {
                int offset = br.ReadInt32();
                br.AssertInt32(new int[1]);
                int count = br.ReadInt32();
                br.AssertInt32(new int[1]);
                Fields = FfxField.ReadManyAt(br, offset, count);
            }

            internal void Write(BinaryWriterEx bw, List<Section10> section10S)
            {
                int count = section10S.Count;
                bw.ReserveInt32(string.Format("Section10Section11sOffset[{0}]", count));
                bw.WriteInt32(0);
                bw.WriteInt32(Fields.Count);
                bw.WriteInt32(0);
                section10S.Add(this);
            }

            internal void WriteSection11S(BinaryWriterEx bw, int index, ref int section11Count)
            {
                bw.FillInt32(string.Format("Section10Section11sOffset[{0}]", index), (int)bw.Position);
                foreach (FfxField field in Fields)
                    field.Write(bw);
                section11Count += Fields.Count;
            }
        }
    }
    public class Fxr3EnhancedSerialization
    {
        public static Fxr3Hoodie XmlToFxr3(XDocument xml)
        {
            XmlSerializer test = new XmlSerializer(typeof(Fxr3Hoodie));
            XmlReader xmlReader = xml.CreateReader();

            return (Fxr3Hoodie)test.Deserialize(xmlReader);
        }
        public static XDocument Fxr3ToXml(Fxr3Hoodie fxr)
        {
            XDocument xDoc = new XDocument();

            using (var xmlWriter = xDoc.CreateWriter())
            {
                var thing = new XmlSerializer(typeof(Fxr3Hoodie));
                thing.Serialize(xmlWriter, fxr);
            }

            return xDoc;
        }
    }
}