using System;
using System.Collections.Generic;
using GDT.Model;

namespace GDT.Utility
{
    public static class ComponentUtility
    {
        private const string Position2DComponentName = "Position2DComponent";

        public static EntityComponent AddPosition2D(Entity entity, float x, float y)
        {
            EntityComponent position2DComponent = new (Position2DComponentName);
            position2DComponent.SetProperty("x", x);
            position2DComponent.SetProperty("y", y);
            entity.Components.Add(position2DComponent);

            return position2DComponent;
        }

        public static (float, float) GetPosition2DFromComponent(Entity entity)
        {
            var component = entity.GetComponent(Position2DComponentName);
            if (component == null)
                throw new ArgumentNullException($"Expected node {entity} to have a {Position2DComponentName}");

            return (component.Get<float>("x"), component.Get<float>("y"));
        }
    }
}