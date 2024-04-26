// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;

// public class MaintenanceDáhboardCtrl : MonoBehaviour
// {
//     public GameObject commentPrefab;
//     public GameObject inforPrefab;

//     void Start() {

//     }

//     void Update()
//     {
//         if(true) { // check condition to render this page
//             List<ObjectComment> comments = DataControler.objectTransforms[DataControler.currentIndex].comments;

//         // Lặp qua danh sách comment
//             foreach (ObjectComment objectComment in comments) {
//                 // Tạo instance mới của prefab
//                 GameObject commentObject = Instantiate(commentPrefab, transform.position, Quaternion.identity);

//                 // Thiết lập đối tượng con là con của đối tượng chứa script
//                 commentObject.transform.SetParent(transform, false);

//                 // Lấy các thành phần con (TextMeshPro)
//                 TextMeshProUGUI createdDayText = commentObject.transform.Find("CreatedDayText").GetComponent<TextMeshProUGUI>();
//                 TextMeshProUGUI contentText = commentObject.transform.Find("ContentText").GetComponent<TextMeshProUGUI>();

//                 // Lấy dữ liệu comment từ danh sách
//                 string createdDay = objectComment.createdDay;
//                 string content = objectComment.content;

//                 // Gán dữ liệu vào thành phần con
//                 createdDayText.text = createdDay;
//                 contentText.text = content;
//             }
//         }
//     }



// }