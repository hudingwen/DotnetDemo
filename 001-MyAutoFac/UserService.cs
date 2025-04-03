namespace MyAutoFac
{
    public class UserService : IUserService
    {
        public string say()
        {
            return "ok";
        }

        [CustomProperty]
        public IPropertyService _propertyService { get; set; }
        public string say2()
        {
            return _propertyService.say();
        }
    }
}
