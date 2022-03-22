using UnityEngine;

namespace Assets.Scripts.DebugHelper
{
    public class CommentBox : MonoBehaviour
    {
        [SerializeField]
        int objId;
        [SerializeField, TextArea(3, 10)]
        string comment;

        public int Id { get { return objId; } set { objId = value; } }

        public void AddComment(string comment)
        {
            this.comment += comment + '\n';
        }
    }
}
