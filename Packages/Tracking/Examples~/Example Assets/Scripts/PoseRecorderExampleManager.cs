using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

namespace Leap.Unity.Examples
{
    public class PoseRecorderExampleManager : MonoBehaviour
    {
        [Header("Count Down")]
        public GameObject countDownGameObject;
        public TextMeshProUGUI countDownText;
        public float recordCountDown = 3;
        public GameObject poseDetectedGameObject;
        public TextMeshProUGUI poseDetectedText;

        [Header("References")]
        public HandPoseRecorder recorder;
        public HandPoseEditor editor;
        public HandPoseDetector detector;
        public HandPoseValidator validator;

        private List<HandPoseScriptableObject> posesRecordedThisSession = new List<HandPoseScriptableObject>();

        bool capturing = false;

        private void Awake()
        {
            recorder.OnPoseSaved += OnPoseSaved;
        }

        void Update()
        {
            if (!capturing)
            {
                HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
                if (detectedPose != null)
                {
                    poseDetectedGameObject.SetActive(true);
                    poseDetectedText.text = "Detected pose: " + detectedPose.name;
                }
                else
                {
                    poseDetectedGameObject.SetActive(false);
                    poseDetectedText.text = "No pose detected";
                }
            }
        }

        public void OnPoseSaved(HandPoseScriptableObject pose)
        {
            editor.handPose = pose;
            editor.gameObject.SetActive(true);

            editor.transform.position = Camera.main.transform.position;
            editor.transform.rotation = Camera.main.transform.rotation;
        }

        public void BeginRecording()
        {
            if (!capturing)
            {
                
                capturing = true;
                StartCoroutine(RecordAfterCountDown());
            }
        }

        IEnumerator RecordAfterCountDown()
        {
            countDownGameObject.SetActive(true);
            float timeLeft = recordCountDown;

            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                countDownText.text = Mathf.CeilToInt(timeLeft).ToString();
                yield return null;
            }
            countDownText.text = "0";

            HandPoseScriptableObject savedPose = recorder.SaveCurrentHandPose();
            countDownText.text = savedPose.name + " saved in " + "Assets/" + recorder.savePath;
            posesRecordedThisSession.Add(savedPose);
            capturing = false;
            detector.SetPosesToDetect(posesRecordedThisSession); 
        }
    }
}