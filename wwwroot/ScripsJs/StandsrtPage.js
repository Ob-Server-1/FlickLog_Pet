document.getElementById("GetAll").addEventListener("click", async () => {

    try {
        const response = await fetch("/api/data/GetCard");

        if (!response.ok) {
            // ✅ Сначала получаем ошибку из тела
            const errorData = await response.json().catch(() => ({}));
            throw new Error(
                `Ошибка: ${response.status} ${response.statusText}. ${JSON.stringify(errorData)}`
            );
        }

        // ✅ Только если всё ок — читаем нормальный ответ
        const result = await response.json();
        console.log("Данные:", result);

        const container = document.getElementById("cards-container");
        if (!container) {
            console.warn("Контейнер с id 'cards-container' не найден");
            return;
        }

        const arrCards = result.data;

        if (!Array.isArray(arrCards)) {
            console.warn("Ожидался массив, но получен:", arrCards);
            return;
        }
        container.innerHTML = "";
        for (let item of arrCards) {
            const card = document.createElement("div");
            card.className = "card";
            card.innerHTML = `
                          <strong>${item.nameFilm}</strong>
                          <p>${item.link}</p>
                          <p>Серия: ${item.serNumber}</p>
                          <p>Дата следующей серии: ${item.dateTime}</p>
                          <p>Статус: ${item.statuc}</p>
                      `;
            container.appendChild(card);
        }
    }
    catch (error) {
        console.error("Ошибка при запросе:", error);
        // Можно показать alert или сообщение на странице
        alert("Не удалось загрузить данные: " + error.message);
    }
});