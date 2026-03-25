let debounceTimer;
let sortBy = "Date";
let sortDir = "asc";
let currentPage = 1;

$(document).ready(function () {
    loadAppointments();

    $("#filterPetName, #filterOwnerName, #filterVetId, #filterTypeId, #filterStartDate, #filterEndDate")
        .on("change keyup", function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                currentPage = 1;
                loadAppointments();
            }, 300);
        });

    $(document).on("click", ".page-btn", function (e) {
        e.preventDefault();
        currentPage = $(this).data("page");
        loadAppointments();
    });

    $(document).on("click", ".sort", function (e) {
        e.preventDefault();
        let col = $(this).data("sort");
        if (sortBy === col) {
            sortDir = (sortDir === "asc") ? "desc" : "asc";
        } else {
            sortBy = col;
            sortDir = "asc";
        }
        loadAppointments();
    });

    $("#clearFilters").on("click", function () {
        $("#filterPetName, #filterOwnerName, #filterVetId, #filterTypeId, #filterStartDate, #filterEndDate").val("");
        currentPage = 1;
        loadAppointments();
    });
});

function loadAppointments() {
    var filters = {
        PetName: $("#filterPetName").val() || null,
        OwnerName: $("#filterOwnerName").val() || null,
        VetId: $("#filterVetId").val() || null,
        TypeId: $("#filterTypeId").val() || null,
        StartDate: $("#filterStartDate").val() || null,
        EndDate: $("#filterEndDate").val() || null,
        page: currentPage,
        sortBy: sortBy,
        sortDir: sortDir
    };

    $("#loadingSpinner").show();

    $.get("/Appointments/Filter", filters, function (data) {
        $("#appointmentsTableContainer").html(data);
        $("#loadingSpinner").hide();
    }).fail(function () {
        $("#loadingSpinner").hide();
        alert("Error loading appointments.");
    });
}
