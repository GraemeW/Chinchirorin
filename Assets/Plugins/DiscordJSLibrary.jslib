mergeInto(LibraryManager.library, {

  PostRollData: function (payload) {
    fetch('/chinchirorin/roll', {method: 'POST', body: UTF8ToString(payload)});
  },

});