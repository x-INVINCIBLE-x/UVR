using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    [Header("Challenge Start UI")]
    [SerializeField] private ChallengeUIDataRegistry uiRegistry;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image iconImage;

    [Header("Challenge Progress UI")]
    [SerializeField] private CanvasGroup progressUI;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Challenge Complete UI")]
    [SerializeField] private GameObject challengeCompleteUI;
    [SerializeField] private TextMeshProUGUI completionText;

    [Header("Challenge Fail UI")]
    [SerializeField] private GameObject challengeFailUI;
    [SerializeField] private TextMeshProUGUI failureText;

    [Header("Challenge Statue")]
    [SerializeField] private Transform statueSpawnTransform;

    private GameObject currentStartStatue = null;

    private ChallengeUIData data;
    private readonly float uiCloseTime = 2f;
    private StringBuilder sb = new();
    private Coroutine progressRoutine;

    private void Start()
    {
        Challenge challenge = ChallengeManager.instance.CurrentChallenge;

        ChallengeManager.instance.OnChallengeChoosen += ChallengeStartUI;
        ChallengeManager.instance.OnChallengeSuccess += ChallengeSuccessUI;
        ChallengeManager.instance.OnChallengeFail += ChallengeFailureUI;

        if (challenge != null)
            ChallengeStartUI(challenge);
    }

    private void ChallengeSuccessUI()
    {
        completionText.text = $"{data.displayName} Challenge Successfully Completed";
        StartCoroutine(UIDisplayRoutine(challengeCompleteUI));

        if (progressRoutine != null)
            StopCoroutine(progressRoutine);
        StartCoroutine(FadeInUI(progressUI, 0.5f));

    }

    private void ChallengeFailureUI()
    {
        failureText.text = $"{data.displayName} Failed";
        StartCoroutine(UIDisplayRoutine(challengeFailUI));

        if (progressRoutine != null)
            StopCoroutine(progressRoutine);
        StartCoroutine(FadeOutUI(progressUI, 0.5f));

    }

    private IEnumerator UIDisplayRoutine(GameObject ui)
    {
        CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = ui.AddComponent<CanvasGroup>();

        ui.SetActive(true);

        yield return StartCoroutine(FadeInUI(canvasGroup, 0.5f));

        yield return new WaitForSeconds(uiCloseTime);

        yield return StartCoroutine(FadeOutUI(canvasGroup, 0.5f));

        ui.SetActive(false);
    }

    private IEnumerator FadeInUI(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOutUI(CanvasGroup canvasGroup, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    private void ChallengeStartUI(Challenge challenge)
    {
        data = uiRegistry.GetUIData(challenge.GetID());

        if (DungeonManager.Instance != null)
        {
            difficultyText.text = DungeonManager.Instance.DifficultyLevel.ToString();
            levelText.text = DungeonManager.Instance.Level.ToString();
        }

        if (currentStartStatue != null)
        {
            Destroy(currentStartStatue);
            currentStartStatue = null;
        }

        currentStartStatue = Instantiate(data.startStatue, statueSpawnTransform.position, Quaternion.identity, statueSpawnTransform);

        if (data == null)
        {
            Debug.LogWarning($"No UI data found for challenge ID: {challenge.GetID()}");
            return;
        }


        nameText.text = data.displayName;

        sb.Clear();
        sb.AppendLine(data.description);
        sb.AppendLine();

        if (data.flavourText != null && data.flavourText.Length > 0)
        {
            sb.AppendLine(data.flavourText);
            sb.AppendLine();
        }

        sb.AppendLine(challenge.GetTechnicalDetail());
        sb.AppendLine();

        descriptionText.text = sb.ToString();
        iconImage.sprite = data.icon;

        StartCoroutine(FadeInUI(progressUI, 0.5f));
        if (progressRoutine != null)
            StopCoroutine(progressRoutine);

        progressRoutine = StartCoroutine(UpdateProgressUI(challenge));
    }

    private IEnumerator UpdateProgressUI(Challenge challenge)
    {
        AttachToPlayer();
        while (true)
        {
            progressText.text = challenge.GetProgressText();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void AttachToPlayer()
    {
        Transform playerTransform = PlayerManager.instance.PlayerOrigin.transform.GetChild(0);

        if (playerTransform != null)
        {
            Debug.Log("pt is not null");
            Transform attatchTransform = playerTransform.Find("Right Controller/Progression Anchor");

            if (attatchTransform != null)
            {
                Debug.Log("at is not null");
                progressUI.transform.parent = attatchTransform;
                progressUI.transform.localPosition = Vector3.zero;
                progressUI.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void OnDestroy()
    {
        ChallengeManager.instance.OnChallengeChoosen -= ChallengeStartUI;
        ChallengeManager.instance.OnChallengeSuccess -= ChallengeSuccessUI;
        ChallengeManager.instance.OnChallengeFail -= ChallengeFailureUI;
    }
}