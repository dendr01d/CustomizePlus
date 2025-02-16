﻿// © Customize+.
// Licensed under the MIT license.

using CustomizePlus.Data.Armature;

using FFXIVClientStructs.FFXIV.Common.Math;

namespace CustomizePlus.Data
{
    /// <summary>
    /// Represents a chunk of editable information about a bone.
    /// </summary>
    public class TransformInfo
    {
        /// <summary>
        /// The container from which this transformation information was retrieved.
        /// </summary>
        private IBoneContainer _sourceContainer;

        public string BoneCodeName { get; }
        public string BoneDisplayName { get; set; }
        public BoneData.BoneFamily BoneFamilyName { get; set; }

        public Vector3 TransformationValue { get; set; }
        public BoneAttribute Attribute { get; }
        public PosingSpace ReferenceFrame { get; }

        private TransformInfo(IBoneContainer container, string codename, BoneAttribute att, PosingSpace ps)
        {
            _sourceContainer = container;
            BoneCodeName = codename;
            Attribute = att;
            ReferenceFrame = ps;

            BoneDisplayName = BoneData.GetBoneDisplayName(BoneCodeName);
            BoneFamilyName = BoneData.GetBoneFamily(BoneCodeName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformInfo"/> class
        /// by referencing values from a model bone. (i.e. instantiating from an armature).
        /// </summary>
        public TransformInfo(IBoneContainer container, ModelBone mb, BoneAttribute att, PosingSpace ps)
            : this(container, mb.BoneName, att, ps)
        {
            BoneTransform bt = mb.GetTransformation();

            TransformationValue = att switch
            {
                BoneAttribute.Position => bt.Translation,
                BoneAttribute.FKPosition => bt.KinematicTranslation,
                BoneAttribute.Rotation => bt.Rotation,
                BoneAttribute.FKRotation => bt.KinematicRotation,
                _ => bt.Scaling
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformInfo"/> class
        /// using raw transformation values and a given codename. (i.e. instantiating from a plain CharacterProfile).
        /// </summary>
        public TransformInfo(IBoneContainer container, string codename, BoneTransform tr, BoneAttribute att, PosingSpace ps)
            : this(container, codename, att, ps)
        {
            TransformationValue = att switch
            {
                BoneAttribute.Position => tr.Translation,
                BoneAttribute.FKPosition => tr.KinematicTranslation,
                BoneAttribute.Rotation => tr.Rotation,
                BoneAttribute.FKRotation => tr.KinematicRotation,
                _ => tr.Scaling
            };
        }

        /// <summary>
        /// Push this transformation info back to its source container, updating it with any changes made
        /// to the information since it was first retrieved.
        /// </summary>
        public void PushChanges(BoneAttribute attribute, bool mirrorChanges)
        {
            _sourceContainer.UpdateBoneTransformValue(this, attribute, mirrorChanges);
        }
    }
}
