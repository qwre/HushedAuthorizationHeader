        private static HttpClient Client { get; set; } = new HttpClient { BaseAddress = new Uri("https://api.production.hushed.com/") };
        public async Task<JObject> Auth(string Username, string Password)
        {
            var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://gk.production.hushed.com/users/signin") { Headers = { { "Authorization", string.Format("Basic {0}", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", Username, Utilities.TransformString(Username, Password))))) } } });
            if (((int)response.StatusCode).Equals(204))
            {
                var content = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

                Responce = new Response
                {
                    Success = true,
                    Data = new 
                    {
                        AccessToken = response.Headers.FirstOrDefault(header => header.Key.Contains("Authorization")).Value.FirstOrDefault().Split(' ')[1]
                    }
                };
            }
            else
            {
                Responce = new Response
                {
                    Success = false
                };
            }
            return await Task.Run(() => JObject.FromObject(Responce));
        }
internal class Utilities
{
    public static string TransformString(string Username, string Password)
    {
        char[] cArr = new char[256];
        "53534a377969656f6b6d5678584b7a70714752710cc3cfee33203af762af7fc4e04a826d3f14834dcc3e7165bcad2282cf081a9d707f762a710df73b4d466e9dc2e9bd420ef656c53534a377969656f6b6d5678584b7a70714752710cc3cfee33203af762af7fc4e04a826d3f14834dcc3e7165bcad2282cf081a9d707f762a7"
            .ToCharArray().CopyTo(cArr, 0);

        string str3 = Password + Username.ToLower();
        char[] cArr2 = str3.ToCharArray();
        var hashSet = new HashSet<int>();
        var arrayList = new List<(int, char, char?)>();

        for (int i = 0; i < cArr2.Length;)
        {
            char c = cArr2[i];
            if (hashSet.Contains(c))
            {
                i += char.IsHighSurrogate(c) ? 2 : 1;
            }
            else
            {
                hashSet.Add(c);
                if (char.IsHighSurrogate(c) && i + 1 < cArr2.Length)
                {
                    arrayList.Add((char.ConvertToUtf32(c, cArr2[i + 1]) % 256, c, cArr2[i + 1]));
                    i += 2;
                }
                else
                {
                    cArr[c % 256] = c;
                    i++;
                }
            }
        }

        int size = 256 + arrayList.Count;
        char[] cArr3 = new char[size];
        int i9 = 0, i11 = 0;

        for (int i = 0; i < size; i++)
        {
            var aVar = arrayList.FirstOrDefault(a => a.Item1 == i);
            if (aVar != default)
            {
                cArr3[i9++] = aVar.Item2;
                cArr3[i9++] = aVar.Item3 ?? '\0';
                i11++;
            }
            else
            {
                cArr3[i9++] = cArr[i - i11];
            }
        }

        SHA256 sha256 = SHA256.Create();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in sha256.ComputeHash(Encoding.UTF8.GetBytes(new string(cArr3) + Password + Username.ToLower())))
        {
            stringBuilder.Append(b.ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}
