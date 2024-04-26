// using UnityEngine;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System;
// using System.Text.RegularExpressions;
// public class ObjectComment
// {
//     public string content;
//     public string createdDay;
// }
// public class ObjectTransform
// {
//     public int id { get; set; }
//     public int index {get; set; }
//     public int? stationId { get; set; }
//     public float positionX { get; set; }
//     public float positionY { get; set; }
//     public float positionZ { get; set; }
//     public float rotationX { get; set; }
//     public float rotationY { get; set; }
//     public float rotationZ { get; set; }
//     public float rotationW { get; set; }
//     public float scaleX { get; set; }
//     public float scaleY { get; set; }
//     public float scaleZ { get; set; }
//     public string maintenanceInstruction { get; set; }
//     public List<ObjectComment> comments;
//     private SensorDevice sensorDevice;

//     public ObjectTransform()
//     {
//         comments = new List<ObjectComment>();
//     }
// }

// public class JsonParser
// {
//     public static List<ObjectTransform> ParseJson(string json)
//     {
//         List<ObjectTransform> objectList = new List<ObjectTransform>();

//         // Xóa khoảng trắng và ký tự xuống dòng
//         json = Regex.Replace(json, @"\s", "");

//         // Xóa dấu ngoặc vuông ở đầu và cuối chuỗi JSON
//         json = json.TrimStart('[').TrimEnd(']');

//         // Tách các đối tượng JSON
//         string[] objectStrings = json.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);

//         foreach (string objectString in objectStrings)
//         {
//             Debug.Log("zzzzzzzzzzz" + objectString);
//             ObjectTransform objectTransform = ConvertToObjectTransform(objectString);
//             objectList.Add(objectTransform);
//         }

//         return objectList;
//     }

//     private static ObjectTransform ConvertToObjectTransform(string objectString)
//     {
//         ObjectTransform objectTransform = new ObjectTransform();

//         // Tách các cặp key-value
//         string[] keyValuePairs = objectString.Split(',');

//         foreach (string keyValuePair in keyValuePairs)
//         {
//             string[] parts = keyValuePair.Split(':');
//             if (parts.Length == 2)
//             {
//                 string key = parts[0].Trim().Trim('"');
//                 string value = parts[1].Trim();

//                 switch (key)
//                 {
//                     case "id":
//                         objectTransform.id = int.Parse(value);
//                         break;
//                     case "index":
//                         objectTransform.index = int.Parse(value);
//                         break;
//                     case "stationId":
//                         if (value == "null")
//                             objectTransform.stationId = null;
//                         else
//                             objectTransform.stationId = int.Parse(value);
//                         break;
//                     case "positionX":
//                         objectTransform.positionX = float.Parse(value);
//                         break;
//                     case "positionY":
//                         objectTransform.positionY = float.Parse(value);
//                         break;
//                     case "positionZ":
//                         objectTransform.positionZ = float.Parse(value);
//                         break;
//                     case "rotationX":
//                         objectTransform.rotationX = float.Parse(value);
//                         break;
//                     case "rotationY":
//                         objectTransform.rotationY = float.Parse(value);
//                         break;
//                     case "rotationZ":
//                         objectTransform.rotationZ = float.Parse(value);
//                         break;
//                     case "rotationW":
//                         objectTransform.rotationW = float.Parse(value);
//                         break;
//                     case "scaleX":
//                         objectTransform.scaleX = float.Parse(value);
//                         break;
//                     case "scaleY":
//                         objectTransform.scaleY = float.Parse(value);
//                         break;
//                     case "scaleZ":
//                         objectTransform.scaleZ = float.Parse(value);
//                         break;
//                     case "maintenanceInstruction":
//                         objectTransform.maintenanceInstruction = value;
//                         break;
//                     case "comments":
//                         objectTransform.comments = ConvertToContents(value);
//                         break;
                    
//                 }
//             }
//         }

//         return objectTransform;
//     }

//     private static List<ObjectComment> ConvertToContents(string contentsString)
//     {
//         List<ObjectComment> contents = new List<ObjectComment>();

//         // Xóa dấu ngoặc vuông ở đầu và cuối chuỗi contents
//         contentsString = contentsString.TrimStart('[').TrimEnd(']');

//         // Tách các đối tượng content
//         string[] contentStrings = contentsString.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);

//         foreach (string contentString in contentStrings)
//         {
//             Debug.Log($"Tesst: {contentString}");
//             ObjectComment content = ConvertToContent(contentString);
//             contents.Add(content);
//         }

//         return contents;
//     }

//     private static ObjectComment ConvertToContent(string contentString)
//     {
//         contentString = contentString.TrimStart('{').TrimEnd('}');
//         string[] parts = contentString.Split(',');

//         ObjectComment objectComment = new ObjectComment();

//         foreach (string part in parts)
//         {
//             string[] fieldValue = part.Split(':');
//             if (fieldValue.Length == 2)
//             {
//                 string fieldName = fieldValue[0].Trim().Trim('"');
//                 string fieldValueString = fieldValue[1].Trim().Trim('"');

//                 switch (fieldName)
//                 {
//                     case "content":
//                         objectComment.content = fieldValueString;
//                         break;

//                     case "createdDay":
//                         objectComment.createdDay = fieldValueString;
//                         break;
//                 }
//             }
//         }

//         return objectComment;
//     }
// }