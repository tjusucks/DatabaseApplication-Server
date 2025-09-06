namespace DbApp.Domain;

public class Exceptions
{
    public class NotFoundException(string message) : Exception(message)
    {
    }

    public class ForbiddenException(string message) : Exception(message)
    {
    }

    public class ValidationException(string message) : Exception(message)
    {
    }

    public class ConflictException(string message) : Exception(message)
    {
    }

    public class UnauthorizedException(string message) : Exception(message)
    {
    }
}
