using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kogler.Standard.EfCore
{
    public static class EntityMappingConfigurationExtensions
    {
        public static PropertyBuilder<TProperty> IsOptional<TProperty>(this PropertyBuilder<TProperty> property)
        {
            return property.IsRequired(false);
        }
    }
}