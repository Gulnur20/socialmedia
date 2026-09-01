namespace socialmedia.Exceptions
{
    public class AgeRestrictionException : Exception
    {
        public AgeRestrictionException(string message) : base(message) { }
    }

    public class DuplicateUsernameException : Exception
    {
        public DuplicateUsernameException(string message) : base(message) { }
    }

    public class AlreadyFollowingException : Exception
    {
        public AlreadyFollowingException(string message) : base(message) { }
    }
}
