import { contextBridge } from 'electron';

// Şimdilik boş — ileride token'ı güvenli şekilde saklamak (keytar vb.)
// veya native dosya diyalogları gibi şeyler eklemek istersen buradan
// window.algoforge üzerinden renderer'a açarsın.
contextBridge.exposeInMainWorld('algoforge', {
  platform: process.platform,
});
