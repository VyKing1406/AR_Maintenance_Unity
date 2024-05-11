using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

public class APICallerHelper : MonoBehaviour
{
    public static async Task<string> GetData(string url)
    {
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

    public static HttpStatusCode PostData(string url, string jsonData)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        var postData = Encoding.ASCII.GetBytes(jsonData);
        request.ContentLength = postData.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(postData, 0, postData.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            return response.StatusCode;
        }
    }


    public static HttpStatusCode PatchData(string url, string jsonData)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "PATCH";
        request.ContentType = "application/json";
        var postData = Encoding.ASCII.GetBytes(jsonData);
        request.ContentLength = postData.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(postData, 0, postData.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            return response.StatusCode;
        }
    }
}