function choose_file() {
    document.getElementById('file-input-button').addEventListener('change', getFileName);
}
const getFileName = (event) => {
    const files = event.target.files;
    const fileName = files[0].name;
    document.getElementById('select-file-text').innerHTML = fileName;
}