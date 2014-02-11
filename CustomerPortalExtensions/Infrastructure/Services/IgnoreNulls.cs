using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omu.ValueInjecter;

namespace CustomerPortal.Infrastructure.Services
{
    public class IgnoreNulls : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return (c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null);
        }
    }
}