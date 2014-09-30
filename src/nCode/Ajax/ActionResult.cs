namespace nCode.Ajax
{
    public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static ActionResult SuccessResult
        {
            get { return new ActionResult() { Success = true }; }
        }
    }
}