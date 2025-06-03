using UnityEngine;
using TMPro;      // for TextMeshPro
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    [TextArea]
    public string questionText;      // The text of the question

    public string[] answerOptions;   // Array of answer options (4 elements)

    [Tooltip("Index of the correct answer (0…3)")]
    public int correctAnswerIndex;
}

public class DialogueManager : MonoBehaviour
{
    [Header("List of questions (5 items)")]
    public Question[] questions;  // Set Size = 5 in the Inspector

    [Header("UI elements (assign in Inspector)")]
    public GameObject dialoguePanel;   // DialoguePanel object
    public TMP_Text questionText;      // TextMeshPro component inside DialoguePanel
    public Button[] answerButtons;     // Array of 4 Buttons (AnswerButton1…4)

    [Header("New door (hinged door)")]
    public DoorHingeController doorController;

    private int currentQuestionIndex = 0;
    private int correctCount = 0;
    private bool isDialogueActive = false;

    private void Start()
    {
        // Initially hide the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Bind each button to OnAnswerSelected
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;  // to correctly capture the index in the lambda
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    // Called externally (e.g. from NPCInteraction) to start the dialogue
    public void StartDialogue()
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogWarning("DialogueManager: No questions assigned!");
            return;
        }

        currentQuestionIndex = 0;
        correctCount = 0;
        isDialogueActive = true;

        // Show the panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Show the first question
        ShowQuestion(currentQuestionIndex);
    }

    // Displays the question with the given index
    private void ShowQuestion(int questionIndex)
    {
        if (questionIndex < 0 || questionIndex >= questions.Length)
        {
            Debug.LogError("DialogueManager: Invalid question index " + questionIndex);
            return;
        }

        // Set question text
        if (questionText != null)
            questionText.text = questions[questionIndex].questionText;

        // Populate answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null && i < questions[questionIndex].answerOptions.Length)
            {
                btnText.text = questions[questionIndex].answerOptions[i];
            }
        }
    }

    // Called when an answer button is clicked (index = 0…3)
    private void OnAnswerSelected(int index)
    {
        if (!isDialogueActive) return;

        // Check if the answer is correct
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            correctCount++;
        }

        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Length)
        {
            // Move to the next question
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            // All questions answered — end dialogue
            EndDialogue();
        }
    }

    // Called when dialogue ends (all 5 questions asked)
    private void EndDialogue()
    {
        isDialogueActive = false;

        // Hide the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Check if enough correct answers were given
        if (correctCount >= 3)
        {
            if (doorController != null)
            {
                doorController.StartOpening();
            }
            else
            {
                Debug.LogWarning("DialogueManager: DoorHingeController is not assigned!");
            }
        }
        else
        {
            Debug.Log($"You only answered {correctCount} questions correctly. The door will not open.");
        }
    }
}
