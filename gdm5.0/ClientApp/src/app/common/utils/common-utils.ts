export class CommonUtil {
   static sortProperties(props){
        props = props.map(item => {
            if(item?.order == null) item.order = 0;
            return item;
        });
        return props.sort(function (a, b) {
            if (a?.order > b?.order) {
              return -1;
            }
            if (a?.order < b?.order) {
              return 1;
            }
            return 0;
          });
    }
}