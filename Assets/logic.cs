using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;

public class logic : MonoBehaviour
{
	public Toggle switchFacil, switchDificl;
	
	public Toggle arte, tv, comida, general, geografia, historia, musica, ciencia, sociedad, deporte;
	public Slider slider;
	
	public Canvas loading, menu, game, finish;
	
	public AudioSource audio;
	public AudioClip clip;
	
	List<Questions> questionsTransalte;
	
	public Camera camera;
	
	public TMPro.TextMeshProUGUI questionText, btn1, btn2, btn3, btn4, timer, score, correctsText, bestPlayer;
	
	public TMPro.TMP_InputField namePlayer;
	
	public Button button1, button2, button3, button4;
	
	string dificult = "hard";
	int querionNumber = 0;
	
	int corrects, globalScore = 0;
	
	public float timeLeft;
	public bool timerOn;
	
    void Start()
	{
		bestPlayer.text = PlayerPrefs.GetString("score", "");
		game.enabled=false;
		finish.enabled=false;
	    switchFacil.onValueChanged.AddListener(delegate {
		    changeDificult("facil");
	    });
	    
	    switchDificl.onValueChanged.AddListener(delegate {
		    changeDificult("dificil");
	    });
	}
    
	void Update() {
		startTime();
	}
    
	public void startGame() {
		loading.enabled = true;
		print("dificult "+dificult);
		string categories = getCategories();
		string limit = "&limit=" + slider.value;
		string diff = "&difficulty=" + dificult;
		
		string url = "https://the-trivia-api.com/api/questions?"+categories+limit+diff;
		
		print(url);
		StartCoroutine(GetText(url));
	}


	string getCategories() {
		string categories = "categories=film_and_tv";
		if(arte.isOn) {
			categories = categories + ",arts_and_literature";
		}
		if(comida.isOn) {
			categories = categories + ",food_and_drink";
		}
		if(general.isOn) {
			categories = categories + ",general_knowledge";
		}
		if(geografia.isOn) {
			categories = categories + ",geography";
		}
		if(historia.isOn) {
			categories = categories + ",history";
		}
		if(musica.isOn) {
			categories = categories + ",music";
		}
		if(ciencia.isOn) {
			categories = categories + ",science";
		}
		if(sociedad.isOn) {
			categories = categories + ",society_and_culture";
		}
		if(deporte.isOn) {
			categories = categories + ",sport_and_leisure";
		}
		
		return categories;
	}
	
	void changeDificult (string type) {
		if(type == "facil" && switchFacil.isOn) {
			print("Facil prendido: ");
			dificult = "easy";
			switchDificl.isOn = false;
		} else if(type == "dificil" && switchDificl.isOn) {
			print("Difil: "+switchDificl.isOn);
			dificult = "hard";
			switchFacil.isOn = false;
		}
	}

	IEnumerator GetText(string url)
    {
	    UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        if (www.isNetworkError)
        {
        	print(www.error);
            Debug.Log(www.error);
        }
        else
        {
	        questionsTransalte = JsonConvert.DeserializeObject <List<Questions>>(www.downloadHandler.text);
	        Gameshow();
        }
    }
    
	void Gameshow() {
		
		changeQuestion();
		timerOn = true;
	    menu.enabled = false;
	    game.enabled = true;
	    loading.enabled = false;
	}
	
	void startTime() {
		if(timerOn) {
			if(timeLeft > 0) {
				if(timeLeft < 2.00100 && timeLeft > 2.0000 ) {
					print("sound");
					audio.PlayOneShot(clip);
				}
				timeLeft -= Time.deltaTime;
				updateTimer(timeLeft);
			} else {
				print("tiempo temrinado");
				querionNumber++;
				changeQuestion();
			}
		}
	}
	
	void updateTimer(float currentTime) {
		currentTime += 1;
		
		float minutes = Mathf.FloorToInt(currentTime / 60);
		float seconds = Mathf.FloorToInt(currentTime % 60);
		
		timer.text = string.Format("{0:00} : {1:00}", minutes, seconds);
	}
	
	
	void changeQuestion() {
		print("querionNumber-1.- " + querionNumber + "questionsTransalte.Count.- "+questionsTransalte.Count);
		
		if(querionNumber == questionsTransalte.Count) {
			timerOn = false;
			menu.enabled = false;
			game.enabled = false;
			loading.enabled = false;
			finish.enabled = true;
			finishGame();
			return;
		}
		
		timeLeft = 30;
		questionsTransalte[querionNumber].incorrectAnswers.Add(questionsTransalte[querionNumber].correctAnswer);
		questionText.text = questionsTransalte[querionNumber].question;
		for (int i = 0; i < questionsTransalte[querionNumber].incorrectAnswers.Count; i++) {
			string temp = questionsTransalte[querionNumber].incorrectAnswers[i];
			int randomIndex = Random.Range(i, questionsTransalte[querionNumber].incorrectAnswers.Count);
			questionsTransalte[querionNumber].incorrectAnswers[i] = questionsTransalte[querionNumber].incorrectAnswers[randomIndex];
			questionsTransalte[querionNumber].incorrectAnswers[randomIndex] = temp;
		}
		
		btn1.text = questionsTransalte[querionNumber].incorrectAnswers[0];
		btn2.text = questionsTransalte[querionNumber].incorrectAnswers[1];
		btn3.text = questionsTransalte[querionNumber].incorrectAnswers[2];
		btn4.text = questionsTransalte[querionNumber].incorrectAnswers[3];

	}
	
	void finishGame() {
		score.text = globalScore.ToString();
		correctsText.text = corrects + " de " + (int)slider.value;
	}
	
	public void ReviewQuestionBtn1() {
		print("Respuesta.- "+btn1.text);
		string response = (string)btn1.text;
		reviewQuestion(response);
	}
	
	public void ReviewQuestionBtn2() {
		print("Respuesta.- "+btn2.text);
		string response = (string)btn2.text;
		reviewQuestion(response);
	}
	
	public void ReviewQuestionBtn3() {
		print("Respuesta.- "+btn3.text);
		string response = (string)btn3.text;
		reviewQuestion(response);
	}
	
	public void ReviewQuestionBtn4() {	
		print("Respuesta.- "+btn4.text);
		string response = (string)btn4.text;
		reviewQuestion(response);
	}
	
	void reviewQuestion(string response) {
		string correct = (string)questionsTransalte[querionNumber].correctAnswer;
		print("correcta.- "+correct);
		querionNumber++;
		if (correct == response) {
			print("correcta");
			corrects++;
			buidScore (timeLeft);
		} else {
			print("incorrecta");
		}
		
		changeQuestion();
	}
	
	public void clickReturn() {
		timerOn = false;
		menu.enabled = true;
		finish.enabled = false;
		game.enabled = false;
	}
	
	void buidScore (float time) {
		int score = 110;
		
		if(time > 25 && time < 30) {
			score = score - 10;
		} else if(time > 20 && time < 25) {
			score = score - 20;
		} else if(time > 15 && time < 20) {
			score = score - 30;
		} else if(time > 10 && time < 15) {
			score = score - 40;
		} else if(time > 5 && time < 10) {
			score = score - 50;
		} else if(time > 0 && time < 5) {
			score = score - 60;
		}
		
		globalScore = score;
	}
	
	public void saveLocally () {
		string score = namePlayer.text + ".- " + globalScore + " puntos";
		PlayerPrefs.SetString("score", score );
		bestPlayer.text = score;
	}
	
    
}

public class Questions
{
	public List<string> incorrectAnswers, tags, regions;
    public string category, id, correctAnswer, question, type, difficulty;

    public bool isNiche;

    // Parameterized Constructor
    // User defined
    public Questions(
        string cat,
        string i,
        string correct,
        string ques,
        string ty,
        string diff,
	    List<string> incorrect,
        List<string> tag,
        List<string> reg
        )
    {
        category = cat;
        id = i;
        correctAnswer = correct;
        question = ques;
        type = ty;
        difficulty = diff;
        incorrectAnswers = incorrect;
        tags = tag;
        regions = reg;
    }

}
