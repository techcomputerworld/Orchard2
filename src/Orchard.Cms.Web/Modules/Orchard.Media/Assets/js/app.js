var root = {
    name: 'Media Library',
    path: ''
}


// define the folder component
var folderComponent = Vue.component('folder', {
    template: '#folder-template',
    props: {
        model: Object
    },
    data: function () {
        return {
            open: false,
            empty: false,
            children: null,
            parent: null
        }
    },
    methods: {
        toggle: function () {
            this.open = !this.open
            var self = this;
            if (this.open && !this.children) {
                $.ajax({
                    url: $('#getFoldersUrl').val() + "?path=" + encodeURIComponent(self.model.path),
                    method: 'GET',
                    success: function (data) {
                        self.children = data;
                        self.empty = data.length == 0;
                    },
                    error: function (error) {
                        emtpy = false;
                        alert(JSON.stringify(error));
                    }
                });
            }
        },
        select: function () {
            mediaApp.selectFolder(this.model);
        }
    }
});

var mediaApp = new Vue({
    el: '#mediaApp',
    data: {
        root: root,
        selectedFolder: root,
        mediaItems: [],
        selectedMedia: null
    },
    computed: {
        isHome: function () {
            return this.selectedFolder == root;
        },
        parents: function () {
            var p = [];
            parent = this.selectedFolder;
            while (parent && parent != root) {
                p.unshift(parent);
                parent = parent.parent;
            }
            return p;
        }
    },
    methods: {
        selectFolder: function (folder) {
            this.selectedFolder = folder;
            this.loadFolder(folder);
        },
        uploadUrl: function () {
            return this.selectedFolder ? $('#uploadFiles').val() + "?path=" + encodeURIComponent(this.selectedFolder.path) : null;
        },
        selectRoot: function () {
            this.selectFolder(this.root);
        },
        loadFolder: function (folder) {
            this.selectedMedia = null;
            var self = this;
            $.ajax({
                url: $('#getMediaItemsUrl').val() + "?path=" + encodeURIComponent(folder.path),
                method: 'GET',
                success: function (data) {
                    data.forEach(function (item) {
                        item.open = false;
                    });
                    self.mediaItems = data;
                },
                error: function (error) {
                    alert(JSON.stringify(error));
                }
            });
        },
        selectMedia: function (media) {
            this.selectedMedia = media;
        },
        deleteFolder: function () {
            var folder = this.selectedFolder
            // The root folder can't be deleted
            if (folder == this.root) {
                return;
            }

            if (!confirm($('#deleteMessage').val())) {
                return;
            }

            $.ajax({
                url: $('#deleteFolderUrl').val() + "?path=" + encodeURIComponent(folder.path),
                method: 'POST',
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                },
                success: function (data) {
                    var parent = folder.$parent;
                    var array = parent.children;
                    array.splice(array.indexOf(folder), 1);
                    mediaApp.selectFolder(parent);
                },
                error: function (error) {
                    alert(JSON.stringify(error));
                }
            });
        }
    }
});
