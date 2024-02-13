namespace MovieStore.Api.MappingProfiles;

public static class Mapper
{
    public static TDestination Map<TSource, TDestination>(TSource sourceObject)
    {
        var destinationObject = Activator.CreateInstance<TDestination>();
        if (sourceObject != null)
        {
            foreach (var sourceProperty in typeof(TSource).GetProperties())
            {
                var destinationProperty = typeof(TDestination).GetProperty(sourceProperty.Name);
                if (destinationProperty != null)
                {
                    destinationProperty.SetValue(destinationObject, sourceProperty.GetValue(sourceObject));
                }
            }
        }
        return destinationObject;
    }
}
