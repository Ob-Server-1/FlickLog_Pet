// Находим карточку-плюс по ID
const addCard = document.getElementById('addCard');

// Находим контейнер
const container = document.querySelector('.cards-container');

// --- ФУНКЦИЯ: ПОКАЗАТЬ ФОРМУ ДОБАВЛЕНИЯ ---
addCard.addEventListener('click', () => {
    // Проверяем: уже ли открыта форма?
    if (document.querySelector('.card-form')) {
        return; // Нельзя открывать две формы
    }

    // Создаём форму
    const formCard = document.createElement('div');
    formCard.className = `card card-form`;
    formCard.dataset.cardId = "";

    formCard.innerHTML = `
        <input type="text" id="nameFilmCard" placeholder="Название фильма" autofocus />
        <input type="text" id="linkCard" placeholder="Ссылка на фильм" />
        <input type="number" min="1" max="100000" id="numberCard" placeholder="Номер серии" />
        <input type="date" id="dateCard" placeholder="Дата выхода следующей серии" />

        <select id="status">
            <option value="pr">Просмотрено</option>
            <option value="sm">Смотрю</option>
            <option value="zb">Заброшено</option>
        </select>

        <div class="card-actions">
            <button type="button" class="btn-ok">✅ ОК</button>
            <button type="button" class="btn-cancel">❌ Отмена</button>
        </div>
    `;

    container.insertBefore(formCard, addCard.nextSibling);
    formCard.querySelector('input').focus();

    // --- КНОПКА "ОК" ---
    formCard.querySelector('.btn-ok').addEventListener('click', async () => {
        // ✅ Собираем данные по id
        const nameFilm = document.getElementById("nameFilmCard").value.trim();
        const link = document.getElementById("linkCard").value.trim(); // ❌ Было linkCard
        const serNumber = document.getElementById("numberCard").value.trim();
        const dateTime = document.getElementById("dateCard").value.trim();
        const statuc = document.getElementById("status").value.trim(); // ❌ Было statusCard

        // ✅ Проверка обязательных полей
        if (!nameFilm) {
            alert('Введите название фильма!');
            return;
        }

        // ✅ Отправляем на бэкенд
        try {
            const response = await fetch("/api/data/add", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json" // ❌ Было aplecation/json
                },
                credentials: "include",
                body: JSON.stringify({
                    nameFilm,
                    link,
                    serNumber,
                    dateTime,
                    statuc
                })
            });

            if (response.ok) {
                const newCard = await response.json();
                console.log("Карточка создана:", newCard);
                alert("Карточка успешно создана");

                // ✅ Создаём карточку на фронтенде
                createCardOnFrontend(newCard); // Передаём данные с бэкенда
                console.log(newCard);
            } else {
                const error = await response.json().catch(() => ({}));
                alert(`Ошибка: ${error.message || 'Не удалось сохранить'}`);
            }
        } catch (error) {
            console.error("Ошибка сети:", error);
            alert("Не удалось подключиться к серверу");
        }

        // ✅ Удаляем форму
        formCard.remove();
    });

    // --- КНОПКА "ОТМЕНА" ---
    formCard.querySelector('.btn-cancel').addEventListener('click', () => {
        formCard.remove();
    });
});

// --- ФУНКЦИЯ: СОЗДАТЬ КАРТОЧКУ НА ФРОНТЕНДЕ ---
function createCardOnFrontend(data) {
    const card = document.createElement('div');
    card.className = 'card';
    card.dataset.cardId = data.id;


    const statucLast = {
        pr: "Просмотренно",
        sm: "Смотрю",
        zb: "Заброшено"
    }[data.statuc] || "Неизвестно!";


   
    card.innerHTML = `
        <strong>${data.nameFilm}</strong>
        <p>${data.link || ''}</p>
        <p>Серия: ${data.serNumber || ''}</p>
        <p>Дата выхода след. серии: ${data.dateTime || ''}</p>
        <p>Статус: ${statucLast || ''}</p>
        <div class="card-actions">
            <button class="edit-btn" aria-label="Редактировать">📝</button>
            <button class="delete-btn" aria-label="Удалить">🗑️</button>
        </div>
    `;

    // Вставляем карточку
    container.insertBefore(card, addCard.nextSibling);

    // ✅ Привязываем события
    attachCardEvents(card);
}

// --- ФУНКЦИЯ: привязать события к карточке ---
function attachCardEvents(card) {
    // --- УДАЛЕНИЕ ---
    card.querySelector('.delete-btn').addEventListener('click', async () => {
        const readCardId = +card.dataset.cardId;

        try {
            const response = await fetch(`/api/data/deleteCard/${readCardId}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include"
            });

            if (!response.ok) {
                throw new Error(`Ошибка: ${response.status}`);
            }

            const result = await response.json();
            console.log("Карточка удалена:", result);
            card.remove();
            alert("Карточка успешно удалена!");
        } catch (error) {
            console.error("Ошибка при удалении:", error);
            alert("Не удалось удалить карточку");
        }
    });

    // --- РЕДАКТИРОВАНИЕ ---
    card.querySelector('.edit-btn').addEventListener('click', () => {
        const idCard = card.dataset.cardId;

        // ✅ Получаем данные из карточки (не из input!)
        const filmName = card.querySelector('strong').textContent;
        const link = card.querySelector('p:nth-of-type(1)').textContent;
        const serNumber = card.querySelector('p:nth-of-type(2)').textContent
            .replace('Серия: ', '')
            .trim();
        const dateTime = card.querySelector('p:nth-of-type(3)').textContent
            .split(': ')[1]; // → "2025-04-10"
        const statuc = card.querySelector('p:nth-of-type(4)')
            .textContent
            .split(': ')[1]
            .trim(); // ✅ Убирает пробелы, табуляцию, переносы

        const editForm = document.createElement('div');
        editForm.className = 'card card-form';
        editForm.dataset.cardId = idCard;

        editForm.innerHTML = `
            <input type="text" value="${filmName}" id="nameFilmCard" placeholder="Название фильма" autofocus />
            <input type="text" value="${link}" id="linkCard" placeholder="Ссылка на фильм" />
            <input type="number" value="${serNumber}" min="1" max="100000" id="numberCard" placeholder="Номер серии" />
            <input type="date" value="${dateTime}" id="dateCard" placeholder="Дата выхода следующей серии" />
            <select id="status">
                <option value="pr" ${statuc === "Просмотрено" ? "selected" : ""}>Просмотрено</option>
                <option value="sm" ${statuc === "Смотрю" ? "selected" : ""}>Смотрю</option>
                <option value="zb" ${statuc === "Заброшено" ? "selected" : ""}>Заброшено</option>
            </select>
            <div class="card-actions">
                <button type="button" class="btn-ok">✅ ОК</button>
                <button type="button" class="btn-cancel">❌ Отмена</button>
            </div>
        `;

        container.replaceChild(editForm, card);

        // --- КНОПКА "ОК" ---
        editForm.querySelector('.btn-ok').addEventListener('click', async () => {
            // ✅ Получаем новые значения из формы
            const newFilmName = document.getElementById("nameFilmCard").value.trim();
            const newLink = document.getElementById("linkCard").value.trim();
            const newSerNumber = document.getElementById("numberCard").value.trim();
            const newDateTime = document.getElementById("dateCard").value;
            const newStatuc = document.getElementById("status").value;

            if (!newFilmName) {
                alert("Введите название фильма!");
                return;
            }

            try {
                const response = await fetch(`/api/data/changeCard/${idCard}`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    credentials: "include",
                    body: JSON.stringify({
                        nameFilm: newFilmName,
                        link: newLink,
                        serNumber: newSerNumber,
                        dateTime: newDateTime,
                        statuc: newStatuc
                    })
                });

                if (response.ok) {
                    const updatedData = await response.json();

                    // ✅ Создаём обновлённую карточку
                    const updatedCard = document.createElement('div');
                    updatedCard.className = 'card';
                    updatedCard.dataset.cardId = Number(updatedData.id);
                    console.log(updatedData.id);
                    // ✅ Текст статуса
                    const statusText = { pr: "Просмотрено", sm: "Смотрю", zb: "Заброшено" }[newStatuc] || "Неизвестно";

                    updatedCard.innerHTML = `
                <strong>${updatedData.nameFilm}</strong>
                <p>${updatedData.link}</p>
                <p>Серия: ${updatedData.serNumber}</p>
                <p>Дата: ${updatedData.dateTime}</p>
                <p>Статус: ${statusText}</p>
                <div class="card-actions">
                    <button class="edit-btn">📝</button>
                    <button class="delete-btn">🗑️</button>
                </div>
            `;

                    // ✅ Возвращаем карточку в DOM
                    container.replaceChild(updatedCard, editForm);

                    // ✅ Привязываем события к новой карточке
                    attachCardEvents(updatedCard);

                    alert("Карточка успешно отредактирована");
                } else {
                    const error = await response.json().catch(() => ({}));
                    alert(`Ошибка при сохранении: ${error.message || "Неизвестная ошибка"}`);
                }
            } catch (error) {
                console.error("Ошибка сети:", error);
                alert("Не удалось подключиться к серверу");
            }
        });

        // --- КНОПКА "ОТМЕНА" ---
        editForm.querySelector('.btn-cancel').addEventListener('click', () => {
            // ✅ Возвращаем старую карточку
            container.replaceChild(card, editForm);
            // События уже есть → не нужно перепривязывать
        });
    }); // ← Закрытие .edit-btn.addEventListener

} // ← Закрытие функции attachCardEvents(card)