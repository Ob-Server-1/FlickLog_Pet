// Находим карточку-плюс по ID
const addCard = document.getElementById('addCard');

// Находим контейнер, куда будем добавлять карточки
const container = document.querySelector('.cards-container');

// Счётчик для уникальных ID карточек (чтобы потом удалять/редактировать)
let cardIdCounter = 0;

// --- ФУНКЦИЯ: ПОКАЗАТЬ ФОРМУ ДОБАВЛЕНИЯ ---
addCard.addEventListener('click', () => {
    // Проверяем: уже ли открыта форма?
    // Нельзя открывать две формы одновременно
    if (document.querySelector('.card-form')) {
        return; // Если есть — выходим
    }

    // Создаём новую карточку-форму
    const formCard = document.createElement('div');
    formCard.className = 'card card-form'; // Классы для стилей

    // Заполняем форму полями и кнопками
    formCard.innerHTML = `
        <input type="text" placeholder="Заголовок" autofocus />
        <input type="text" placeholder="Описание" />
        <div class="card-actions">
            <button type="button" class="btn-ok">✅ ОК</button>
            <button type="button" class="btn-cancel">❌ Отмена</button>
        </div>
    `;

    /*
        ВАЖНО: вставляем форму СРАЗУ ПОСЛЕ карточки-плюса.
        То есть: [ + ] [ Форма ] [ Старые карточки... ]
        Это гарантирует, что плюс всегда остаётся первым.
    */
    container.insertBefore(formCard, addCard.nextSibling);

    // Фокус на первое поле
    formCard.querySelector('input').focus();

    // --- КНОПКА "ОК" ---
    formCard.querySelector('.btn-ok')
    .addEventListener('click', () => {
        // Получаем значения из полей
        const title = formCard.querySelector('input:nth-of-type(1)').value.trim();
        const desc = formCard.querySelector('input:nth-of-type(2)').value.trim();

        // Проверяем, что заголовок не пустой
        if (!title) {
            alert('Введите заголовок!');
            return;
        }

        // Создаём новую карточку с данными
        createCard(title, desc);

        // Удаляем форму
        formCard.remove();
    });

    // --- КНОПКА "ОТМЕНА" ---
    formCard.querySelector('.btn-cancel')
    .addEventListener('click', () => {
        formCard.remove(); // Просто удаляем форму
    });
});

// --- ФУНКЦИЯ: СОЗДАТЬ НОВУЮ КАРТОЧКУ ---
function createCard(title, description) {
    const card = document.createElement('div');
    card.className = 'card';
    card.dataset.id = ++cardIdCounter;

    card.innerHTML = `
        <strong>${title}</strong>
        <p>${description || ''}</p>
        <div class="card-actions">
            <button class="edit-btn" aria-label="Редактировать">📝</button>
            <button class="delete-btn" aria-label="Удалить">🗑️</button>
        </div>
    `;

    // Вставляем карточку
    container.insertBefore(card, addCard.nextSibling);

    // Привязываем события — ВАЖНО: делаем это здесь
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