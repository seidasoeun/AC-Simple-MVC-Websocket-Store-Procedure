using System;
using System.Reflection;

namespace simpleMvc.Api5.StoreProcedure.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}