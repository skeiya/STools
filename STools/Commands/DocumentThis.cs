﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using EnvDTE;
using System.Windows.Forms;
using System.Globalization;


namespace S2.STools.Commands
{
    class DocumentThis : ICommand
    {

        public bool IsYourId(uint commandId)
        {
            return commandId == PkgCmdIDList.CommandIdDocumentThis;
        }

        public bool IsEnable(DTE dte)
        {
            CodeModel cm = dte.ActiveDocument.ProjectItem.ContainingProject.CodeModel; ;
            return cm.Language == CodeModelLanguageConstants.vsCMLanguageVC;
        }

        public void Execute(DTE dte)
        {
            CodeFunction func = GetSelectedFunction(dte);
            if (func == null)
            {
                MessageBox.Show(Resources.FunctionNotSelected);
                return;
            }
            func.StartPoint.CreateEditPoint().Insert(GetFuncCommen(func));
        }

        private static CodeFunction GetSelectedFunction(DTE dte)
        {
            CodeModel cm = dte.ActiveDocument.ProjectItem.ContainingProject.CodeModel;
            foreach (CodeElement e in cm.CodeElements)
            {
                if (e.Kind != vsCMElement.vsCMElementFunction) continue;
                CodeFunction func = (CodeFunction)e;
                if (IsFuncSelected(dte, func)) return func;
            }
            return null;
        }

        private static bool IsFuncSelected(DTE dte, CodeFunction func)
        {
            int selLine = ((TextSelection)dte.ActiveDocument.Selection).AnchorPoint.Line;
            return func.StartPoint.Line <= selLine
                                && selLine <= func.EndPoint.Line;
        }

        private string GetFuncCommen(CodeFunction func)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"///<summary>" + GetDescriptionFromCamelcase(func.Name) + @"</summary>" + Environment.NewLine);
            foreach (Tuple<string, string> param in GetParamNamesAndTypes(func))
            {
                str.Append(@"///<param name='" + param.Item1 + "'>" + GetDescriptionFromCamelcase(param.Item2) + @"</param>" + Environment.NewLine);
            }
            return str.ToString(); ;
        }

        private static string GetDescriptionFromCamelcase(string funcName)
        {
            StringBuilder buff = new StringBuilder();
            bool isFirst = true;
            foreach (char c in funcName.ToCharArray())
            {
                if (isFirst)
                {
                    isFirst = false;
                    buff.Append(Char.ToUpper(c, CultureInfo.CurrentCulture));
                }
                else
                {
                    if (Char.IsUpper(c))
                    {
                        buff.Append(" ");
                        buff.Append(Char.ToLower(c, CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        buff.Append(c);
                    }
                }
            }
            buff.Append(".");

            return buff.ToString();
        }

        private IEnumerable<Tuple<string, string>> GetParamNamesAndTypes(CodeFunction func)
        {
            foreach (CodeParameter param in func.Parameters)
            {
                yield return new Tuple<string, string>(param.FullName, param.Type.AsString);
            }
        }
    }
}
