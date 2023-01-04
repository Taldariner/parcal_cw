namespace TCP_server;

public class TestQuestion
{
    public string Question;
    public int RightAnswer;

    public TestQuestion(string question, int rightAnswer)
    {
        Question = question;
        RightAnswer = rightAnswer;
    }
    
}

public class Test
{
    private Queue<TestQuestion> _testQuestions;
    private TestQuestion _currentQuestion;
    public int Score { get; private set; }

    public Test()
    {
        _testQuestions = new Queue<TestQuestion>();
        _testQuestions.Enqueue(new TestQuestion("У під'їзді шістнадцятиповерхового будинку на першому поверсі розташовано 6 квартир, а на кожному з решти поверхів – по 8. На якому поверсі квартира № 31, якщо квартири від № 1 і далі пронумеровано послідовно від першого до останнього поверху?\n1) 3\n2) 4\n3) 5\n4) 6\n", 3));
        _testQuestions.Enqueue(new TestQuestion("Точки A та B лежать на сфері радіуса 10 см. Укажіть найбільше можливе значення довжини відрізка AB.\n1) 20 см\n2) 100π см\n3) 10 см\n4) 20π см\n", 1));
        _testQuestions.Enqueue(new TestQuestion("Обчисліть суму коренів рівняння x^2 + 3x - 4 = 0\n1) -4\n2) -3 \n3) 3\n4) 4\n", 2));
        _testQuestions.Enqueue(new TestQuestion("Укажіть функцію, графік якої проходить через початок координат.\n1) y = x-1\n2) y = 1-x \n3) y = 1 \n4) y = x\n", 4));
        _testQuestions.Enqueue(new TestQuestion("Спростіть вираз 2(x+5y)-(4y-7x) \n1) 9x+y\n2) 9x+14y \n3) -5x+6y\n4) 9x+6y\n", 4));
        _testQuestions.Enqueue(new TestQuestion("За 6 однакових конвертів заплатили 3 грн. Скільки всього таких конвертів можна купити за 12 грн?\n1) 6\n2) 24 \n3) 30\n4) 36\n", 2));
        _testQuestions.Enqueue(new TestQuestion("Укажіть корінь рівняння 1 - 5x = 0 \n1) 5\n2) -1/5 \n3) 1/5 \n4) 4\n", 3));
        _testQuestions.Enqueue(new TestQuestion("Сума трьох кутів паралелограма дорівнює 280°. Визначте градусну міру більшого кута цього паралелограма. \n1) 100°\n2) 80° \n3) 140° \n4) 40°\n", 1));
        _testQuestions.Enqueue(new TestQuestion("Розв’яжіть рівняння x/10 = 2.5 \n1) 0.25\n2) 4 \n3) 12.5 \n4) 25\n", 4));
        _testQuestions.Enqueue(new TestQuestion("(a-4)^2-a^2 =  \n1) -8a+16 \n2) 8a+16 \n3) 16 \n4) -4a+16\n", 1));
    }        

    
    public string PeekQuestion()
    {
        _currentQuestion = _testQuestions.Dequeue();
        return _currentQuestion.Question;
    }

    public void CheckQuestion(int clientAnswer)
    {
        if (clientAnswer == _currentQuestion.RightAnswer)
        {
            Score++;
        }
    }

    public int GetQuestionsCount()
    {
        return _testQuestions.Count;
    }
}