using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;

public class DialogueUGUI : MonoBehaviour {

	[System.Serializable]
	public class SubtitleDelays
	{
		public float characterDelay = 0.05f;
		public float sentenceDelay  = 0.5f;
		public float commaDelay     = 0.1f;
		public float finalDelay     = 1.2f;
	}


	//Group...
	public RectTransform subtitlesGroup;
	public Text actorSpeech;
	public Text actorName;
	public Image actorPortrait;
	public SubtitleDelays subtitleDelays = new SubtitleDelays();

	//Group...
	public RectTransform optionsGroup;
	public Button optionButton;
	private Dictionary<Button, int> cachedButtons;

	void OnEnable(){
		DialogueTree.OnDialogueStarted       += OnDialogueStarted;
		DialogueTree.OnDialoguePaused        += OnDialoguePaused;
		DialogueTree.OnDialogueFinished      += OnDialogueFinished;
		DialogueTree.OnSubtitlesRequest      += OnSubtitlesRequest;
		DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
	}

	void Start(){
		subtitlesGroup.gameObject.SetActive(false);
		optionsGroup.gameObject.SetActive(false);
		optionButton.gameObject.SetActive(false);
	}

	void OnDialogueStarted(DialogueTree dlg){
		//nothing special...
	}

	void OnDialoguePaused(DialogueTree dlg){
		subtitlesGroup.gameObject.SetActive(false);
		optionsGroup.gameObject.SetActive(false);
	}

	void OnDialogueFinished(DialogueTree dlg){
		subtitlesGroup.gameObject.SetActive(false);
		optionsGroup.gameObject.SetActive(false);
	}


	void OnSubtitlesRequest(SubtitlesRequestInfo info){
		StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
	}

	IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info){

		actorSpeech.text = "";
		subtitlesGroup.gameObject.SetActive(true);
		
		actorName.text = info.actor.name;
		actorSpeech.color = info.actor.dialogueColor;
		
		actorPortrait.gameObject.SetActive( info.actor.portraitSprite != null );
		actorPortrait.sprite = info.actor.portraitSprite;

		var text = "";
		for (int i= 0; i < info.statement.text.Length; i++){
			
			if (subtitlesGroup.gameObject.activeSelf == false)
				yield break;

			text += info.statement.text[i];
			yield return new WaitForSeconds(subtitleDelays.characterDelay);
			char c = info.statement.text[i];
			if (c == '.' || c == '!' || c == '?')
				yield return new WaitForSeconds(subtitleDelays.sentenceDelay);
			if (c == ',')
				yield return new WaitForSeconds(subtitleDelays.commaDelay);

			actorSpeech.text = text;
		}

		yield return new WaitForSeconds(subtitleDelays.finalDelay);

		subtitlesGroup.gameObject.SetActive(false);
		info.Continue();
	}

	void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info){

		var buttonHeight = optionButton.GetComponent<RectTransform>().rect.height;
		var groupTransform = optionsGroup.GetComponent<RectTransform>();
		optionsGroup.gameObject.SetActive(true);
		groupTransform.sizeDelta = new Vector2(groupTransform.rect.width, (info.options.Values.Count * buttonHeight) + 5);

		cachedButtons = new Dictionary<Button, int>();
		int i = 0;

		foreach (KeyValuePair<IStatement, int> pair in info.options){
			var btn = (Button)Instantiate(optionButton);
			btn.gameObject.SetActive(true);
			btn.transform.SetParent(optionsGroup.transform, false);
			btn.transform.localPosition = (Vector2)optionButton.transform.localPosition - new Vector2(0, buttonHeight * i);
			btn.GetComponentInChildren<Text>().text = pair.Key.text;
			cachedButtons.Add(btn, pair.Value);
			btn.onClick.AddListener( ()=> { Finalize(info, cachedButtons[btn]);	});
			i++;
		}
	}


	void Finalize(MultipleChoiceRequestInfo info, int index){
		optionsGroup.gameObject.SetActive(false);
		foreach (var tempBtn in cachedButtons.Keys)
			Destroy(tempBtn.gameObject);			
		info.SelectOption(index);
	}
}