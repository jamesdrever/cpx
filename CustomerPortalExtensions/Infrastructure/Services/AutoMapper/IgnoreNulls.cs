using Omu.ValueInjecter;

namespace CustomerPortalExtensions.Infrastructure.Services.AutoMapper
{
    public class IgnoreNulls : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return (c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null);
        }
    }
}