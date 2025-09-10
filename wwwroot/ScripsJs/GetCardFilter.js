// ✅ Обработчик: сортировка по дате
document.getElementById("FilterDate").addEventListener("click", function () {
    const isActive = this.classList.toggle("active");
    this.dataset.active = isActive;
});

// ✅ Переместил обработчики кнопок статуса ВНЕ обработчика "Поиск"
const statucButtons = document.querySelectorAll(".statuc-btn");
statucButtons.forEach(button => {
    button.addEventListener("click", function () {
        statucButtons.forEach(btn => {
            btn.classList.remove("active");
            btn.dataset.active = "false";
        });
        this.classList.add("active");
        this.dataset.active = "true";
    });
});

// ✅ Обработчик: поиск
document.getElementById("search").addEventListener("click", async () => {
    const search = document.getElementById("searchText").value;

    const params = new URLSearchParams();

    // ✅ Исправлен селектор: .statuc-btn.active
    const activeBtn = document.querySelector(".statuc-btn.active");
    const filter = activeBtn?.dataset.filter || ""; // ✅ data-filter, не data-statuc

    if (filter) {
        params.append("sortStatuc", filter);
    }

    if (search) {
        params.append("search", search);
    }

    const filterBtn = document.getElementById("FilterDate");
    if (filterBtn?.dataset.active === "true") {
        params.append("sortData", "desc");
    }

    const url = `/api/data/GetCard?${params}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Ошибка: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();
        const container = document.getElementById("cards-container");
        if (!container) {
            console.warn("Контейнер с id 'cards-container' не найден");
            return;
        }

        container.innerHTML = ''; // Очищаем

        const arrCards = result.data;
        if (!Array.isArray(arrCards)) {
            console.warn("Ожидался массив, но получен:", arrCards);
            return;
        }

        for (let item of arrCards) {
            const card = document.createElement("div");
            const statucLast = {
                pr: "Просмотренно",
                sm: "Смотрю",
                zb: "Заброшено"
            }[item.statuc] || "Неизвестно!";

            card.className = "card";
            card.innerHTML = `
                            <strong>${item.nameFilm}</strong>
                            <p>${item.link}</p>
                            <p>Серия: ${item.serNumber}</p>
                            <p>Дата следующей серии: ${item.dateTime}</p>
                            <p>Статус: ${statucLast}</p>
                        `;
            container.appendChild(card);
        }
    } catch (error) {
        console.error("Ошибка при запросе:", error);
        alert("Не удалось загрузить данные: " + error.message);
    }
});