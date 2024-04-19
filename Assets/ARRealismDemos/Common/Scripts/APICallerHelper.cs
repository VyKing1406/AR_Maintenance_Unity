using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class APICallerHelper : MonoBehaviour
{
    public static async Task<string> GetData(string url)
    {
        //Debug.Log(url);
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return data;
            }
            else
            {
                Debug.LogError("API request failed with status code: " + response.StatusCode);
                return null; // Hoặc trả về giá trị mặc định khác tùy vào yêu cầu của bạn
            }
        }
    }
}