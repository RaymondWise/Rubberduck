﻿using System.Collections;
using System.Collections.Generic;
using Rubberduck.VBEditor.SafeComWrappers.VB.Abstract;
using VB6IA = Microsoft.VB6.Interop.VBIDE;

namespace Rubberduck.VBEditor.SafeComWrappers.VB.VB6
{
    public class CodePanes : SafeComWrapper<VB6IA.CodePanes>, ICodePanes
    {
        public CodePanes(VB6IA.CodePanes target) 
            : base(target)
        {
        }

        public int Count => IsWrappingNullReference ? 0 : Target.Count;

        public IVBE Parent => new VBE(IsWrappingNullReference ? null : Target.Parent);

        public IVBE VBE => new VBE(IsWrappingNullReference ? null : Target.VBE);

        public ICodePane Current 
        { 
            get => new CodePane(IsWrappingNullReference ? null : Target.Current);
            set => Target.Current = (VB6IA.CodePane)value.Target;
        }

        public ICodePane this[object index] => new CodePane(Target.Item(index));

        IEnumerator<ICodePane> IEnumerable<ICodePane>.GetEnumerator()
        {
            return new ComWrapperEnumerator<ICodePane>(Target, o => new CodePane((VB6IA.CodePane)o));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ICodePane>)this).GetEnumerator();
        }

        //public override void Release(bool final = false)
        //{
        //    if (!IsWrappingNullReference)
        //    {
        //        for (var i = 1; i <= Count; i++)
        //        {
        //            this[i].Release();
        //        }
        //        base.Release(final);
        //    }
        //}

        public override bool Equals(ISafeComWrapper<VB6IA.CodePanes> other)
        {
            return IsEqualIfNull(other) || (other != null && ReferenceEquals(other.Target, Target));
        }

        public bool Equals(ICodePanes other)
        {
            return Equals(other as SafeComWrapper<VB6IA.CodePanes>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : Target.GetHashCode();
        }
    }
}