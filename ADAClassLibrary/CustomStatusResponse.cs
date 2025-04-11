namespace ADAClassLibrary
{
    public class CustomStatusResponse
    {
        public static Response GetResponse(int StatusCode)
        {
            if (StatusCode == 200) return new Response()
            {
                Status = 200
            };
            else if (StatusCode == 401) return new Response()
            {
                Status = 401,
                ResponseMsg = "Unauthorized access"
            };
            else if (StatusCode == 403) return new Response()
            {
                Status = 403,
                ResponseMsg = "You don’t have permission to this action."
            };
            else if (StatusCode == 320) return new Response()
            {
                Status = 320,
                ResponseMsg = "User with these credential not exist."
            };
            else if (StatusCode == 500) return new Response()
            {
                Status = 500,
                ResponseMsg = "Internal server error"
            };
            else if (StatusCode == 600) return new Response()
            {
                Status = 600
            };
            else if (StatusCode == 700) return new Response()
            {
                Status = 700,
            };
            else return new Response()
            {
                Status = 500,
                ResponseMsg = "Internal server error Error"
            };
        }
    }
}
