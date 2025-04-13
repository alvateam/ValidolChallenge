using UnityEngine;
using UnityEngine.UI;

namespace Source.Scripts
{
    public class TikTokScroll : MonoBehaviour
    {
        public ScrollRect scrollRect; // Ссылка на ScrollRect
        public float snapSpeed = 8f;  // Скорость защелкивания
        public float[] pagePositions; // Массив позиций для каждого элемента

        private int currentPage = 0;  // Текущая страница
        private bool isScrolling = false; // Флаг для отслеживания прокрутки

        void Update()
        {
            if (!isScrolling)
            {
                // Интерполируем позицию ScrollRect к ближайшей странице
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(
                    scrollRect.verticalNormalizedPosition,
                    pagePositions[currentPage],
                    Time.deltaTime * snapSpeed
                );
            }
        }

        public void OnDragEnd()
        {
            // Определяем ближайшую страницу после окончания прокрутки
            float closestPosition = Mathf.Infinity;
            int newPage = currentPage;

            for (int i = 0; i < pagePositions.Length; i++)
            {
                float distance = Mathf.Abs(scrollRect.verticalNormalizedPosition - pagePositions[i]);
                if (distance < closestPosition)
                {
                    closestPosition = distance;
                    newPage = i;
                }
            }

            currentPage = newPage; // Обновляем текущую страницу
        }

        public void OnScrollStart()
        {
            isScrolling = true; // Пользователь начал прокручивать
        }

        public void OnScrollEnd()
        {
            isScrolling = false; // Пользователь закончил прокручивать
            OnDragEnd();         // Вызываем метод для защелкивания
        }
    }
}