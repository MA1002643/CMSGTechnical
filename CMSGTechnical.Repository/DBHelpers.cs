using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CMSGTechnical.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Repository
{
    internal static class DBHelpers
    {

        public static void FindMissingEntitiesOfType<TMissingType>(this ModelBuilder builder, Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                    .Where(i => i is { IsAbstract: false, IsGenericType: false })
                    .Where(i => i.IsAssignableTo(typeof(TMissingType)))
                ;

            foreach (var type in types)
            {
                if (builder.Model.FindEntityType(type)!=null)
                    continue;

                var entity = builder.Entity(type);
                entity.Property(nameof(IEntity.Id))
                    .ValueGeneratedOnAdd();
                entity.HasKey(nameof(IEntity.Id));
            }
        }

    }
}
