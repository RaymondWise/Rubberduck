﻿using Rubberduck.VBEditor.SafeComWrappers.VB.Abstract;
using VBAIA = Microsoft.Vbe.Interop;

namespace Rubberduck.VBEditor.SafeComWrappers.VB.VBA
{
    public class AddIn : SafeComWrapper<VBAIA.AddIn>, IAddIn
    {
        public AddIn(Microsoft.Vbe.Interop.AddIn target) 
            : base(target)
        {
        }

        public string ProgId => IsWrappingNullReference ? string.Empty : Target.ProgId;

        public string Guid => IsWrappingNullReference ? string.Empty : Target.Guid;

        public IVBE VBE => new VBE(IsWrappingNullReference ? null : Target.VBE);

        public IAddIns Collection => new AddIns(IsWrappingNullReference ? null : Target.Collection);

        public string Description
        {
            get => IsWrappingNullReference ? string.Empty : Target.Description;
            set { if (!IsWrappingNullReference) Target.Description = value; }
        }

        public bool Connect
        {
            get => !IsWrappingNullReference && Target.Connect;
            set { if (!IsWrappingNullReference) Target.Connect = value; }
        }

        public object Object // definitely leaks a COM object
        {
            get => IsWrappingNullReference ? null : Target.Object;
            set { if (!IsWrappingNullReference) Target.Object = value; }
        }

        public override bool Equals(ISafeComWrapper<VBAIA.AddIn> other)
        {
            return IsEqualIfNull(other) || (other != null && other.Target.ProgId == ProgId && other.Target.Guid == Guid);
        }

        public bool Equals(IAddIn other)
        {
            return Equals(other as SafeComWrapper<VBAIA.AddIn>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Compute(ProgId, Guid);
        }
    }
}