using System;
using System.IO;
using System.Linq;
using Rubberduck.VBEditor.Extensions;
using Rubberduck.VBEditor.SafeComWrappers.VB.Abstract;
using Rubberduck.VBEditor.SafeComWrappers.Office.Abstract;
using Rubberduck.VBEditor.SafeComWrappers.VB.Enums;
using VB6IA = Microsoft.VB6.Interop.VBIDE;

namespace Rubberduck.VBEditor.SafeComWrappers.VB.VB6
{
    public class VBComponent : SafeComWrapper<VB6IA.VBComponent>, IVBComponent
    {
        public VBComponent(VB6IA.VBComponent target) 
            : base(target)
        {
        }

        public ComponentType Type => IsWrappingNullReference ? 0 : (ComponentType)Target.Type;

        public ICodeModule CodeModule => new CodeModule(IsWrappingNullReference ? null : Target.CodeModule);

        public IVBE VBE => new VBE(IsWrappingNullReference ? null : Target.VBE);

        public IVBComponents Collection => new VBComponents(IsWrappingNullReference ? null : Target.Collection);

        public IProperties Properties => new Properties(IsWrappingNullReference ? null : Target.Properties);

        public bool HasOpenDesigner => !IsWrappingNullReference && Target.HasOpenDesigner;

        public string DesignerId => IsWrappingNullReference ? string.Empty : Target.DesignerID;

        public string Name
        {
            get => IsWrappingNullReference ? string.Empty : Target.Name;
            set => Target.Name = value;
        }

        public IControls Controls => throw new NotImplementedException();

        public IControls SelectedControls => throw new NotImplementedException();

        public bool HasDesigner
        {
            get
            {
                if (IsWrappingNullReference)
                {
                    return false;
                }
                var designer = Target.Designer;
                var hasDesigner = designer != null;
                return hasDesigner;
            }
        }

        public IWindow DesignerWindow()
        {
            return new Window(IsWrappingNullReference ? null : Target.DesignerWindow());
        }

        public void Activate()
        {
            Target.Activate();
        }

        public bool IsSaved => throw new NotImplementedException();

        public void Export(string path)
        {
            //Target.Export(path);
        }

        /// <summary>
        /// Exports the component to the folder. The file name matches the component name and file extension is based on the component's type.
        /// </summary>
        /// <param name="folder">Destination folder for the resulting source file.</param>
        /// <param name="tempFile">True if a unique temp file name should be generated. WARNING: filenames generated with this flag are not persisted.</param>
        public string ExportAsSourceFile(string folder, bool tempFile = false)
        {
            var fullPath = tempFile
                ? Path.Combine(folder, Path.GetRandomFileName())
                : Path.Combine(folder, Name + Type.FileExtension());
            switch (Type)
            {
                case ComponentType.UserForm:
                    ExportUserFormModule(fullPath);
                    break;
                case ComponentType.Document:
                    ExportDocumentModule(fullPath);
                    break;
                default:
                    Export(fullPath);
                    break;
            }

            return fullPath;
        }

        public IVBProject ParentProject { get; private set; }

        private void ExportUserFormModule(string path)
        {
            // VBIDE API inserts an extra newline when exporting a UserForm module.
            // this issue causes forms to always be treated as "modified" in source control, which causes conflicts.
            // we need to remove the extra newline before the file gets written to its output location.

            var visibleCode = CodeModule.Content().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var legitEmptyLineCount = visibleCode.TakeWhile(string.IsNullOrWhiteSpace).Count();

            var tempFile = ExportToTempFile();
            var contents = File.ReadAllLines(tempFile);
            var nonAttributeLines = contents.TakeWhile(line => !line.StartsWith("Attribute")).Count();
            var attributeLines = contents.Skip(nonAttributeLines).TakeWhile(line => line.StartsWith("Attribute")).Count();
            var declarationsStartLine = nonAttributeLines + attributeLines + 1;

            var emptyLineCount = contents.Skip(declarationsStartLine - 1)
                                         .TakeWhile(string.IsNullOrWhiteSpace)
                                         .Count();

            var code = contents;
            if (emptyLineCount > legitEmptyLineCount)
            {
                code = contents.Take(declarationsStartLine).Union(
                       contents.Skip(declarationsStartLine + emptyLineCount - legitEmptyLineCount))
                               .ToArray();
            }
            File.WriteAllLines(path, code);
        }

        private void ExportDocumentModule(string path)
        {
            var lineCount = CodeModule.CountOfLines;
            if (lineCount > 0)
            {
                var text = CodeModule.GetLines(1, lineCount);
                File.WriteAllText(path, text);
            }
        }

        private string ExportToTempFile()
        {
            var path = Path.Combine(Path.GetTempPath(), Name + Type.FileExtension());
            Export(path);
            return path;
        }
        //public override void Release(bool final = false)
        //{
        //    if (!IsWrappingNullReference)
        //    {
        //        DesignerWindow().Release();
        //        Controls.Release();
        //        Properties.Release();
        //        CodeModule.Release();
        //        base.Release(final);
        //    }
        //}

        public override bool Equals(ISafeComWrapper<VB6IA.VBComponent> other)
        {
            return IsEqualIfNull(other) || (other != null && ReferenceEquals(other.Target, Target));
        }

        public bool Equals(IVBComponent other)
        {
            return Equals(other as SafeComWrapper<VB6IA.VBComponent>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : Target.GetHashCode();
        }
    }
}