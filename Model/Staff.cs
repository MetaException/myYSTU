namespace myYSTU.Model
{
    public class Staff
    {
        public string Name { get; set; }
        public string Post { get; set; }

        //TODO: хранить в базе данных ссылки на изображения, а для офлайн доступа - изображения
        public ImageSource Avatar { get; set; }
        public string AvatarUrl { get; set; }
    }
}
