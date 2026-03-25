//$(document).ready(function () {
//    let currentDuration = parseInt($("#AppointmentTypeId option:selected").data("duration")) || 0;

//    function updateDateTime() {
//        const date = $("#AppointmentDate").val();
//        const hour = $("#AppointmentHour").val();
//        const minute = $("#AppointmentMinute").val();

//        if (date && hour !== "" && minute !== "" && currentDuration > 0) {
//            const combined = `${date}T${String(hour).padStart(2, "0")}:${String(minute).padStart(2, "0")}`;
//            $("#AppointmentDateTime").val(combined);

//            const start = new Date(combined);
//            const end = new Date(start.getTime() + currentDuration * 60000);

//            $("#SummaryStart").text(start.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }));
//            $("#SummaryDuration").text(currentDuration);
//            $("#SummaryEnd").text(end.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }));
//            $("#AppointmentSummary").show();
//        } else {
//            $("#AppointmentSummary").hide();
//        }
//    }

//    function updateType() {
//        const appointmentTypeId = $("#AppointmentTypeId").val();
//        if (!appointmentTypeId) {
//            $("#Price, #PriceHidden").val("");
//            currentDuration = 0;
//            updateDateTime();
//            return;
//        }

//        $.getJSON("/Appointments/GetPrice", { appointmentTypeId }, function (data) {
//            const price = data?.price != null ? parseFloat(data.price).toFixed(2) : "0.00";
//            currentDuration = data?.durationMinutes || 0;

//            $("#Price, #PriceHidden").val(price);
//            updateDateTime();
//            fetchAvailableTimes();
//        });
//    }

//    function fetchAvailableTimes() {
//        const date = $("#AppointmentDate").val();
//        const vetId = $("#VeterinarianId").val();
//        const appointmentTypeId = $("#AppointmentTypeId").val();
//        if (!date || !appointmentTypeId) return;

//        const url = vetId ? "/Appointments/GetAvailableTimes" : "/Appointments/GetAvailableTimesForClinic";

//        $.getJSON(url, { date, veterinarianId: vetId, appointmentTypeId }, function (slots) {
//            const hourDropdown = $("#AppointmentHour").empty();
//            const minuteDropdown = $("#AppointmentMinute").empty();

//            if (!slots || !slots.length) {
//                hourDropdown.append('<option value="">No slots</option>');
//                minuteDropdown.append('<option value="">--</option>');
//                return;
//            }

//            const hours = [...new Set(slots.map(s => s.split(":")[0]))];
//            const minutes = [...new Set(slots.map(s => s.split(":")[1]))];

//            hours.forEach(h => hourDropdown.append(`<option value="${h}">${h}</option>`));
//            minutes.forEach(m => minuteDropdown.append(`<option value="${m}">${m}</option>`));

//            // Preselect first slot if empty
//            if (!$("#AppointmentHour").val()) $("#AppointmentHour").val(hours[0]);
//            if (!$("#AppointmentMinute").val()) $("#AppointmentMinute").val(minutes[0]);

//            updateDateTime();
//        });
//    }

//    // Event bindings
//    $("#AppointmentTypeId").change(updateType);
//    $("#AppointmentDate, #AppointmentHour, #AppointmentMinute, #VeterinarianId").change(function () {
//        updateDateTime();
//        fetchAvailableTimes();
//    });

//    // Initial load
//    updateType();
//});






$(document).ready(function () {

    function getCombinedDateTime() {
        var date = $("#AppointmentDate").val();
        var hour = $("#AppointmentHour").val();
        var minute = $("#AppointmentMinute").val();

        if (date && hour !== null && minute !== null) {
            return new Date(date + "T" + hour.padStart(2, '0') + ":" + minute.padStart(2, '0'));
        }
        return null;
    }

    function updateEndTime(duration) {
        var appointmentDateTime = getCombinedDateTime();

        if (appointmentDateTime && duration) {
            var end = new Date(appointmentDateTime);
            end.setMinutes(end.getMinutes() + duration);

            var hours = end.getHours().toString().padStart(2, '0');
            var minutes = end.getMinutes().toString().padStart(2, '0');
            $("#EndTime").val(hours + ':' + minutes);
        } else {
            $("#EndTime").val('');
        }
    }

    $("#AppointmentTypeId").change(function () {
        var appointmentTypeId = $(this).val();

        if (appointmentTypeId) {
            $.ajax({
                url: '/Appointments/GetPrice',
                type: 'GET',
                data: { appointmentTypeId: appointmentTypeId },
                success: function (data) {
                    $("#Price").val(data.price);
                    updateEndTime(data.durationMinutes);
                },
                error: function () {
                    $("#Price").val('');
                    $("#EndTime").val('');
                    alert("Could not fetch price.");
                }
            });
        } else {
            $("#Price").val('');
            $("#EndTime").val('');
        }
    });

    $("#AppointmentDate, #AppointmentHour, #AppointmentMinute").change(function () {
        var duration = parseInt($("#AppointmentTypeId option:selected").data("duration")) || 0;
        updateEndTime(duration);
    });

    // Trigger once on page load
    $("#AppointmentTypeId").trigger('change');
});




//$(document).ready(function () {
//    // Populate hour/minute dropdowns
//    for (let h = 8; h <= 18; h++) {
//        $("#AppointmentHour").append(new Option(h.toString().padStart(2, '0'), h));
//    }
//    for (let m = 0; m < 60; m += 15) {
//        $("#AppointmentMinute").append(new Option(m.toString().padStart(2, '0'), m));
//    }

//    function updateDateTimeHidden() {
//        let date = $("#AppointmentDate").val();
//        let hour = $("#AppointmentHour").val();
//        let minute = $("#AppointmentMinute").val();

//        if (date && hour !== null && minute !== null) {
//            let fullDateTime = `${date}T${hour.padStart(2, '0')}:${minute.padStart(2, '0')}:00`;
//            $("#AppointmentDateTime").val(fullDateTime);
//            return fullDateTime;
//        }
//        return null;
//    }

//    function updateEndTime(duration) {
//        let startVal = updateDateTimeHidden();
//        if (startVal && duration) {
//            let start = new Date(startVal);
//            start.setMinutes(start.getMinutes() + duration);

//            let hours = start.getHours().toString().padStart(2, '0');
//            let minutes = start.getMinutes().toString().padStart(2, '0');
//            $("#EndTime").val(hours + ':' + minutes);
//        } else {
//            $("#EndTime").val('');
//        }
//    }

//    $("#AppointmentTypeId").change(function () {
//        var appointmentTypeId = $(this).val();

//        if (appointmentTypeId) {
//            $.ajax({
//                url: '/Appointments/GetPrice',
//                type: 'GET',
//                data: { appointmentTypeId: appointmentTypeId },
//                success: function (data) {
//                    $("#Price").val(parseFloat(data.price).toFixed(2));
//                    $("#PriceHidden").val(parseFloat(data.price).toFixed(2));
//                    updateEndTime(data.durationMinutes);
//                },
//                error: function () {
//                    $("#Price").val('');
//                    $("#PriceHidden").val('');
//                    $("#EndTime").val('');
//                    alert("Could not fetch price.");
//                }
//            });
//        } else {
//            $("#Price").val('');
//            $("#PriceHidden").val('');
//            $("#EndTime").val('');
//        }
//    });

//    $("#AppointmentDate, #AppointmentHour, #AppointmentMinute").change(function () {
//        var duration = parseInt($("#AppointmentTypeId option:selected").data("duration")) || 0;
//        updateEndTime(duration);
//    });

//    // Trigger once on page load
//    $("#AppointmentTypeId").trigger('change');
//});
