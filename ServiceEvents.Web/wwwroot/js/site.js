// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.querySelectorAll(".event-create-form").forEach(form => {
    const formatDate = digits => {
        let value = digits.slice(0, 2);
        if (digits.length > 2) value += "." + digits.slice(2, 4);
        if (digits.length > 4) value += "." + digits.slice(4, 8);
        if (digits.length > 8) value += " " + digits.slice(8, 10);
        if (digits.length > 10) value += ":" + digits.slice(10, 12);
        return value;
    };

    const parseDate = value => {
        const match = /^(\d{2})\.(\d{2})\.(\d{4}) (\d{2}):(\d{2})$/.exec(value);
        if (!match) return null;

        const [, day, month, year, hour, minute] = match;
        const date = new Date(Number(year), Number(month) - 1, Number(day), Number(hour), Number(minute));
        if (date.getFullYear() !== Number(year)
            || date.getMonth() !== Number(month) - 1
            || date.getDate() !== Number(day)
            || date.getHours() !== Number(hour)
            || date.getMinutes() !== Number(minute)) {
            return null;
        }

        return date;
    };

    form.querySelectorAll(".event-date-field").forEach(field => {
        const textInput = field.querySelector(".event-date-input");
        const picker = field.querySelector(".event-date-picker");
        const calendarButton = field.querySelector(".event-calendar-button");

        textInput.addEventListener("input", () => {
            const cursor = textInput.selectionStart ?? textInput.value.length;
            const digitsBeforeCursor = textInput.value.slice(0, cursor).replace(/\D/g, "").length;
            const digits = textInput.value.replace(/\D/g, "").slice(0, 12);
            textInput.value = formatDate(digits);

            let nextCursor = 0;
            let countedDigits = 0;
            while (nextCursor < textInput.value.length && countedDigits < digitsBeforeCursor) {
                if (/\d/.test(textInput.value[nextCursor])) countedDigits++;
                nextCursor++;
            }
            textInput.setSelectionRange(nextCursor, nextCursor);

            const date = parseDate(textInput.value);
            if (date) {
                const pad = number => String(number).padStart(2, "0");
                picker.value = `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
            }
        });

        calendarButton.addEventListener("click", () => {
            const currentDate = parseDate(textInput.value);
            if (currentDate) {
                const pad = number => String(number).padStart(2, "0");
                picker.value = `${currentDate.getFullYear()}-${pad(currentDate.getMonth() + 1)}-${pad(currentDate.getDate())}T${pad(currentDate.getHours())}:${pad(currentDate.getMinutes())}`;
            }

            if (typeof picker.showPicker === "function") {
                try {
                    picker.showPicker();
                    return;
                } catch (error) {
                    if (!(error instanceof DOMException)
                        || !["NotAllowedError", "InvalidStateError"].includes(error.name)) {
                        throw error;
                    }

                    field.classList.add("picker-fallback");
                }
            } else {
                field.classList.add("picker-fallback");
            }

            picker.focus();
        });

        picker.addEventListener("change", () => {
            if (!picker.value) return;
            const [datePart, timePart] = picker.value.split("T");
            const [year, month, day] = datePart.split("-");
            const [hour, minute] = timePart.split(":");
            textInput.value = `${day}.${month}.${year} ${hour}:${minute}`;
            textInput.dispatchEvent(new Event("input", { bubbles: true }));
        });
    });

    form.addEventListener("submit", () => {
        const startDate = parseDate(form.querySelector('[name="StartDate"]').value);
        const endDate = parseDate(form.querySelector('[name="EndDate"]').value);
        form.querySelector('[name="startDateOffsetMinutes"]').value =
            startDate ? startDate.getTimezoneOffset() : "";
        form.querySelector('[name="endDateOffsetMinutes"]').value =
            endDate ? endDate.getTimezoneOffset() : "";
    });
});
