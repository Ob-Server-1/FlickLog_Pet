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
    formCard.className = 'card card-form';

    formCard.innerHTML = `
        <input type="text" id="nameFilmCard" placeholder="Название фильма" autofocus />
        <input type="text" id="linkCard" placeholder="Ссылка на фильм" />
        <input type="text" id="numberCard" placeholder="Номер серии" />
        <input type="text" id="dateCard" placeholder="Дата выхода следующей серии" />

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
    const statucLast = {
        pr: "Просмотренно",
        sm: "Смотрю",
        zb: "Заброшено"
    }[data.statuc] || "Неизвестно!";


    


    card.innerHTML = `
        <strong>${data.nameFilm}</strong>
        <p>${data.link || ''}</p>
        <p>Серия: ${data.serNumber || ''}</p>
        <p>Дата: ${data.dateTime || ''}</p>
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
    // Удаление
    card.querySelector('.delete-btn').addEventListener('click', () => {
        card.remove();
    });

    // Редактирование
    card.querySelector('.edit-btn').addEventListener('click', () => {
        const currentTitle = card.querySelector('strong').textContent;
        const currentDesc = card.querySelector('p').textContent;

        const editForm = document.createElement('div');
        editForm.className = 'card card-form';
        editForm.dataset.id = card.dataset.id;

        editForm.innerHTML = `
            <input type="text" value="${currentTitle}" />
            <input type="text" value="${currentDesc}" />
            <div class="card-actions">
                <button type="button" class="save-btn">✅ Сохранить</button>
                <button type="button" class="cancel-edit">❌ Отмена</button>
            </div>
        `;

        container.replaceChild(editForm, card);

        // --- КНОПКА "СОХРАНИТЬ" ---
        editForm.querySelector('.save-btn').addEventListener('click', () => {
            const newTitle = editForm.querySelector('input:nth-of-type(1)').value.trim();
            const newDesc = editForm.querySelector('input:nth-of-type(2)').value.trim();

            if (!newTitle) {
                alert('Введите заголовок!');
                return;
            }

            // Создаём новую карточку
            const updatedCard = document.createElement('div');
            updatedCard.className = 'card';
            updatedCard.dataset.id = editForm.dataset.id;
            updatedCard.innerHTML = `
                <strong>${newTitle}</strong>
                <p>${newDesc}</p>
                <div class="card-actions">
                    <button class="edit-btn">📝</button>
                    <button class="delete-btn">🗑️</button>
                </div>
            `;

            // Возвращаем карточку
            container.replaceChild(updatedCard, editForm);

            // ✅ Привязываем события к обновлённой карточке
            attachCardEvents(updatedCard);
        });

        // --- КНОПКА "ОТМЕНА" ---
        editForm.querySelector('.cancel-edit').addEventListener('click', () => {
            container.replaceChild(card, editForm);
            // События у `card` уже есть — не нужно перепривязывать
        });
    });
}