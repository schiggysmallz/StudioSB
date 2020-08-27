using System.Collections.Generic;
using OpenTK;
using SSBHLib;
using SSBHLib.Formats;

namespace StudioSB.Scenes.Ultimate
{
    public class SKEL_Loader
    {
        public static SBSkeleton Open(string FileName, SBScene Scene)
        {
            SsbhFile File;
            if (Ssbh.TryParseSsbhFile(FileName, out File))
            {
                if (File is Skel skel)
                {
                    var Skeleton = new SBSkeleton();
                    Scene.Skeleton = Skeleton;

                    Dictionary<int, SBBone> idToBone = new Dictionary<int, SBBone>();
                    Dictionary<SBBone, int> needParent = new Dictionary<SBBone, int>();
                    foreach (var b in skel.BoneEntries)
                    {
                        SBBone bone = new SBBone();
                        bone.Name = b.Name;
                        bone.Type = b.Type;
                        bone.Transform = Skel_to_TKMatrix(skel.Transform[b.Id]);
                        idToBone.Add(b.Id, bone);
                        if (b.ParentId == -1)
                            Skeleton.AddRoot(bone);
                        else
                            needParent.Add(bone, b.ParentId);
                    }
                    foreach(var v in needParent)
                    {
                        v.Key.Parent = idToBone[v.Value];
                    }

                    return Skeleton;
                }
            }
            return null;
        }

        public static void Save(string FileName, SBScene Scene)
        {
            var Skeleton = Scene.Skeleton;

            var skelFile = new Skel();

            skelFile.MajorVersion = 1;
            skelFile.MinorVersion = 0;

            List<SkelBoneEntry> BoneEntries = new List<SkelBoneEntry>();
            List<SkelMatrix> Transforms = new List<SkelMatrix>();
            List<SkelMatrix> InvTransforms = new List<SkelMatrix>();
            List<SkelMatrix> WorldTransforms = new List<SkelMatrix>();
            List<SkelMatrix> InvWorldTransforms = new List<SkelMatrix>();

            short index = 0;
            Dictionary<SBBone, short> BoneToIndex = new Dictionary<SBBone, short>();
            var OrderedBones = SortBones(Skeleton.Bones);

            foreach (var bone in OrderedBones)
            {
                BoneToIndex.Add(bone, index);
                var boneentry = new SkelBoneEntry();
                boneentry.Name = bone.Name;
                boneentry.Type = bone.Type;
                boneentry.Id = index++;
                boneentry.Type = 1;
                boneentry.ParentId = -1;
                if (bone.Parent != null)// && BoneToIndex.ContainsKey(bone.Parent))
                    boneentry.ParentId = (short)OrderedBones.IndexOf(bone.Parent);
                BoneEntries.Add(boneentry);

                Transforms.Add(TKMatrix_to_Skel(bone.Transform));
                InvTransforms.Add(TKMatrix_to_Skel(bone.Transform.Inverted()));
                WorldTransforms.Add(TKMatrix_to_Skel(bone.WorldTransform));
                InvWorldTransforms.Add(TKMatrix_to_Skel(bone.InvWorldTransform));
            }

            skelFile.BoneEntries = BoneEntries.ToArray();
            skelFile.Transform = Transforms.ToArray();
            skelFile.InvTransform = InvTransforms.ToArray();
            skelFile.WorldTransform = WorldTransforms.ToArray();
            skelFile.InvWorldTransform = InvWorldTransforms.ToArray();

            Ssbh.TrySaveSsbhFile(FileName, skelFile);
        }

        private static string[] FighterBoneSet = {
            "Trans",
            "Rot",
            "Hip",
            "LegC",
            "LegL",
            "KneeL",
            "FootL",
            "ToeL",
            "LegR",
            "KneeR",
            "FootR",
            "ToeR",
            "Waist",
            "Bust",
            "Neck",
            "Head",
            "H_BoneFixTest01",
            "H_BoneFixTest02",
            "H_BoneFixTest03",
            "H_BoneFixTest04",
            "H_BoneFixTest05",
            "H_BoneFixTest06",
            "H_BoneFixTest07",
            "H_BoneFixTest08",
            "H_BoneFixTest09",
            "H_BoneFixTest10",
            "H_BoneFixTest11",
            "H_BoneFixTest12",
            "H_BoneFixTest13",
            "H_BoneFixTest14",
            "H_BoneFixTest15",
            "H_BoneFixTest16",
            "H_BoneFixTest17",
            "H_BoneFixTest18",
            "H_BoneFixTest19",
            "H_BoneFixTest20",
            "H_BoneFixTest21",
            "H_BoneFixTest22",
            "H_BoneFixTest23",
            "H_BoneFixTest24",
            "H_BoneFixTest25",
            "H_BoneFixTest26",
            "H_BoneFixTest27",
            "H_BoneFixTest28",
            "H_BoneFixTest29",
            "H_BoneFixTest30",
            "H_BoneFixTest31",
            "H_BoneFixTest32",
            "H_BoneFixTest33",
            "H_BoneFixTest34",
            "H_BoneFixTest35",
            "H_BoneFixTest36",
            "H_BoneFixTest37",
            "H_BoneFixTest38",
            "H_BoneFixTest39",
            "H_BoneFixTest40",
            "H_BoneFixTest41",
            "H_BoneFixTest42",
            "Hat",
            "Hair_top",
            "ClavicleC",
            "ClavicleL",
            "ShoulderL",
            "ArmL",
            "HandL",
            "HaveL",
            "ClavicleR",
            "ShoulderR",
            "ArmR",
            "HandR",
            "HaveR",
            "VestFFR",
            "VestFR",
            "VestFFL",
            "VestFL",
            "VestBL",
            "VestBR",
            "CollarB",
            "CollarBL",
            "CollarBR",
            "Throw",
            "S_HairB1",
            "S_HairB2",
            "S_HairB3",
            "S_HairB4",
            "S_HairB5_null",
            "S_HairL1",
            "S_HairL2_null",
            "S_HairR1",
            "S_HairR2_null",
            "S_HairF1",
            "S_HairF2_null",
            "S_SleeveL",
            "S_SleeveL_null",
            "S_SleeveBL",
            "S_SleeveBL_null",
            "S_SleeveFL",
            "S_SleeveFL_null",
            "S_SleeveFR",
            "S_SleeveFR_null",
            "S_SleeveR",
            "S_SleeveR_null",
            "S_SleeveBR",
            "S_SleeveBR_null",
            "S_CollarL",
            "S_CollorL1_null",
            "S_CollarR",
            "S_CollorR1_null",
            "S_VestFFR1",
            "S_VestFFR2",
            "S_VestFFR3_null",
            "S_VestFR1",
            "S_VestFR2",
            "S_VestFR3_null",
            "S_VestFFL1",
            "S_VestFFL2",
            "S_VestFFL3_null",
            "S_VestFL1",
            "S_VestFL2",
            "S_VestFL3_null",
            "S_VestBL1",
            "S_VestBL2",
            "S_VestBL3_null",
            "S_VestBR1",
            "S_VestBR2",
            "S_VestBR3_null",
            "S_VestBBR1",
            "S_VestBBR2",
            "S_VestBBR3_null",
            "S_VestBBL1",
            "S_VestBBL2",
            "S_VestBBL_3_null",
            "Face",
            "Mouth_group",
            "UplipL1",
            "UplipR1",
            "UplipL2",
            "UplipR2",
            "UplipC",
            "Jaw",
            "Tongue",
            "DownlipC",
            "DownlipR",
            "DownlipL",
            "FingerL10",
            "FingerL20",
            "FingerL30",
            "FingerL40",
            "FingerL41",
            "FingerL42",
            "FingerL43",
            "FingerL31",
            "FingerL32",
            "FingerL33",
            "FingerL21",
            "FingerL22",
            "FingerL23",
            "FingerL11",
            "FingerL12",
            "FingerL13",
            "FingerL51",
            "FingerL52",
            "FingerL53",
            "FingerR10",
            "FingerR20",
            "FingerR30",
            "FingerR40",
            "FingerR41",
            "FingerR42",
            "FingerR43",
            "FingerR31",
            "FingerR32",
            "FingerR33",
            "FingerR21",
            "FingerR22",
            "FingerR23",
            "FingerR11",
            "FingerR12",
            "FingerR13",
            "FingerR51",
            "FingerR52",
            "FingerR53",
            "UplipL1_offset",
            "UplipR1_offset",
            "UplipL2_offset",
            "UplipR2_offset",
            "UplipC_offset",
            "Jaw_offset",
            "Tongue_offset",
            "DownlipC_offset",
            "DownlipR_offset",
            "DownlipL_offset",
            "H_KneeL",
            "H_LegL",
            "H_KneeR",
            "H_LegR",
            "H_Neck",
            "H_ArmtwistL",
            "H_WristL",
            "H_ElbowL",
            "H_ShoulderL",
            "H_ArmtwistR",
            "H_WristR",
            "H_ElbowR",
            "H_ShoulderR",
        };

        private static List<SBBone> SortBones(IEnumerable<SBBone> boneList)
        {
            var basic = new List<SBBone>();
            var swing = new List<SBBone>();
            var extra = new List<SBBone>();
            var helper = new List<SBBone>();

            // first collect basic strings
            var copy = new List<SBBone>(boneList);

            foreach(var s in FighterBoneSet)
            {
                foreach(var b in copy)
                {
                    if(b.Name.Equals(s))
                    {
                        basic.Add(b);
                        break;
                    }
                }
            }
            foreach(var b in basic)
                copy.Remove(b);

            // every other bone
            foreach(SBBone b in copy)
            {
                if (b.Name.StartsWith("S_"))
                    swing.Add(b);
                else if (b.Name.StartsWith("H_"))
                    helper.Add(b);
                else
                    extra.Add(b);
            }

            var finalList = new List<SBBone>();
            finalList.AddRange(basic);
            finalList.AddRange(swing);
            finalList.AddRange(extra);
            finalList.AddRange(helper);

            return finalList;
        }

        private static Matrix4 Skel_to_TKMatrix(SkelMatrix sm)
        {
            return new Matrix4(sm.M11, sm.M12, sm.M13, sm.M14,
                sm.M21, sm.M22, sm.M23, sm.M24,
                sm.M31, sm.M32, sm.M33, sm.M34,
                sm.M41, sm.M42, sm.M43, sm.M44);
        }

        private static SkelMatrix TKMatrix_to_Skel(Matrix4 sm)
        {
            var skelmat = new SkelMatrix();
            skelmat.M11 = sm.M11;
            skelmat.M12 = sm.M12;
            skelmat.M13 = sm.M13;
            skelmat.M14 = sm.M14;
            skelmat.M21 = sm.M21;
            skelmat.M22 = sm.M22;
            skelmat.M23 = sm.M23;
            skelmat.M24 = sm.M24;
            skelmat.M31 = sm.M31;
            skelmat.M32 = sm.M32;
            skelmat.M33 = sm.M33;
            skelmat.M34 = sm.M34;
            skelmat.M41 = sm.M41;
            skelmat.M42 = sm.M42;
            skelmat.M43 = sm.M43;
            skelmat.M44 = sm.M44;
            return skelmat;
        }
    }
}
