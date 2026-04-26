namespace FamilyCompass.Application.Families.Exceptions;

public class FamilyNotFoundException(Guid familyId) : Exception($"Family '{familyId}' was not found.");
